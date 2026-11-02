
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace MovieTicketBooking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingService(
            IBookingRepository bookingRepository,
            UserManager<ApplicationUser> userManager)
        {
            _bookingRepository = bookingRepository;
            _userManager = userManager;
        }

        // Create a booking for the authenticated customer.
        public async Task<BookingResponse> CreateBookingAsync(
            CreateBookingRequest request,
            string userId)
        {
            if (request.ShowId <= 0)
            {
                throw new ArgumentException("Invalid show ID.");
            }

            if (request.SeatIds == null || request.SeatIds.Count == 0)
            {
                throw new ArgumentException(
                    "Select at least one seat.");
            }

            if (request.SeatIds.Any(id => id <= 0))
            {
                throw new ArgumentException(
                    "Seat IDs must be greater than zero.");
            }

            if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
            {
                throw new ArgumentException(
                    "Duplicate seat IDs are not allowed.");
            }

            // Retrieve the logged-in customer from Identity.
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException(
                    "Customer account not found.");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new InvalidOperationException(
                    "Customer email is not available.");
            }

            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                throw new InvalidOperationException(
                    "Customer phone number is not available.");
            }

            bool showExists =
                await _bookingRepository.ShowExistsAsync(request.ShowId);

            if (!showExists)
            {
                throw new KeyNotFoundException("Show not found.");
            }

            decimal? ticketPrice =
                await _bookingRepository.GetShowTicketPriceAsync(
                    request.ShowId);

            if (ticketPrice == null)
            {
                throw new KeyNotFoundException("Show not found.");
            }

            var availableSeats =
                await _bookingRepository.GetAvailableShowSeatsAsync(
                    request.ShowId,
                    request.SeatIds);

            if (availableSeats.Count != request.SeatIds.Count)
            {
                throw new InvalidOperationException(
                    "One or more selected seats are invalid or already booked.");
            }

            // Each seat has the show's ticket price in Version 1.
            decimal totalAmount =
                ticketPrice.Value * request.SeatIds.Count;

            var booking = new Booking
            {
                BookingReference = "BK" +
                    Guid.NewGuid().ToString("N")[..20].ToUpper(),

                ShowId = request.ShowId,

                // Link the booking to the authenticated customer.
                ApplicationUserId = user.Id,

                // Customer details come from the Identity account.
                CustomerName = user.FullName.Trim(),

                CustomerEmail = user.Email.Trim(),

                CustomerPhone = user.PhoneNumber.Trim(),

                BookingDate = DateTime.UtcNow,

                TotalAmount = totalAmount,

                Status = BookingStatus.Confirmed,

                BookingSeats = availableSeats.Select(seat =>
                    new BookingSeat
                    {
                        SeatId = seat.SeatId,
                        Price = ticketPrice.Value
                    }).ToList()
            };

            // Preserve the existing repository transaction and seat allocation.
            var createdBooking =
                await _bookingRepository.CreateBookingAsync(
                    booking,
                    request.SeatIds);

            return MapToResponse(createdBooking);
        }

        // Retrieve a booking only if the user owns it or is Admin.
        public async Task<BookingResponse?> GetByIdAsync(
            int bookingId,
            string userId,
            bool isAdmin)
        {
            if (bookingId <= 0)
            {
                throw new ArgumentException("Invalid booking ID.");
            }

            var booking =
                await _bookingRepository.GetByIdAsync(bookingId);

            if (booking == null)
            {
                return null;
            }

            if (!isAdmin && booking.ApplicationUserId != userId)
            {
                return null;
            }

            return MapToResponse(booking);
        }

        // Cancel a booking only if the user owns it or is Admin.
        public async Task<BookingResponse?> CancelBookingAsync(
            int bookingId,
            string userId,
            bool isAdmin)
        {
            if (bookingId <= 0)
            {
                throw new ArgumentException("Invalid booking ID.");
            }

            // Check ownership before cancelling or releasing seats.
            var existingBooking =
                await _bookingRepository.GetByIdAsync(bookingId);

            if (existingBooking == null)
            {
                return null;
            }

            if (!isAdmin && existingBooking.ApplicationUserId != userId)
            {
                return null;
            }

            // Preserve the existing cancellation implementation.
            var booking =
                await _bookingRepository.CancelBookingAsync(bookingId);

            if (booking == null)
            {
                return null;
            }

            return MapToResponse(booking);
        }

        // Retrieve only the bookings belonging to a particular customer.
        public async Task<IEnumerable<BookingResponse>>
            GetBookingsByUserIdAsync(string userId)
        {
            var bookings =
                await _bookingRepository.GetBookingsByUserIdAsync(userId);

            return bookings.Select(MapToResponse).ToList();
        }

        private static BookingResponse MapToResponse(Booking booking)
        {
            return new BookingResponse
            {
                BookingId = booking.BookingId,
                BookingReference = booking.BookingReference,
                ShowId = booking.ShowId,
                CustomerName = booking.CustomerName,
                CustomerEmail = booking.CustomerEmail,
                CustomerPhone = booking.CustomerPhone,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status,

                Seats = booking.BookingSeats.Select(bs =>
                    new BookingSeatResponse
                    {
                        SeatId = bs.SeatId,
                        SeatLabel = bs.Seat == null
                            ? string.Empty
                            : bs.Seat.RowName + bs.Seat.SeatNumber,
                        SeatType = bs.Seat?.SeatType ?? string.Empty,
                        Price = bs.Price
                    }).ToList()
            };
        }
    }
}
