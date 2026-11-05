
using Microsoft.AspNetCore.Identity;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

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

        // 1. Create a new booking
        public async Task<BookingResponse> CreateBookingAsync(
            CreateBookingRequest request,
            string userId)
        {
            if (request.ShowId <= 0)
                throw new ArgumentException("Invalid show ID.");

            if (request.SeatIds == null || !request.SeatIds.Any())
                throw new ArgumentException("Please select at least one seat.");

            if (request.SeatIds.Any(id => id <= 0))
                throw new ArgumentException("Invalid seat ID.");

            if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
                throw new ArgumentException("Duplicate seats are not allowed.");

            // Get the logged-in customer
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new InvalidOperationException("User not found.");

            if (string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                throw new InvalidOperationException(
                    "Customer email and phone number are required.");
            }

            // Check whether the show exists
            var showExists = await _bookingRepository.ShowExistsAsync(
                request.ShowId);

            if (!showExists)
                throw new ArgumentException("Show not found.");

            // Get ticket price
            var ticketPrice = await _bookingRepository.GetShowTicketPriceAsync(
                request.ShowId);

            if (ticketPrice == null)
                throw new InvalidOperationException(
                    "Unable to retrieve ticket price.");

            // Check seat availability
            var availableSeats =
                await _bookingRepository.GetAvailableShowSeatsAsync(
                    request.ShowId,
                    request.SeatIds);

            if (availableSeats.Count != request.SeatIds.Count)
                throw new InvalidOperationException(
                    "One or more selected seats are unavailable.");

            // Calculate total amount
            decimal totalAmount = ticketPrice.Value * request.SeatIds.Count;

            // Create booking object
            var booking = new Booking
            {
                ShowId = request.ShowId,
                ApplicationUserId = user.Id,
                CustomerName = user.FullName,
                CustomerEmail = user.Email,
                CustomerPhone = user.PhoneNumber,
                BookingDate = DateTime.Now,
                TotalAmount = totalAmount,
                Status = BookingStatus.Confirmed
            };

            // Save booking and allocate seats
            var createdBooking =
                await _bookingRepository.CreateBookingAsync(
                    booking,
                    request.SeatIds);

            return MapToResponse(createdBooking);
        }

        // 2. Get the actual booking entity for authorization
        public async Task<Booking?> GetBookingForAuthorizationAsync(
            int bookingId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            return await _bookingRepository.GetByIdAsync(bookingId);
        }

        // 3. Get booking details after authorization succeeds
        public async Task<BookingResponse?> GetByIdAsync(int bookingId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _bookingRepository.GetByIdAsync(bookingId);

            if (booking == null)
                return null;

            return MapToResponse(booking);
        }

        // 4. Cancel booking after authorization succeeds
        public async Task<BookingResponse?> CancelBookingAsync(int bookingId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking =
                await _bookingRepository.CancelBookingAsync(bookingId);

            if (booking == null)
                return null;

            return MapToResponse(booking);
        }

        // 5. Get all bookings belonging to the logged-in customer
        public async Task<IEnumerable<BookingResponse>> GetBookingsByUserIdAsync(
            string userId)
        {
            var bookings =
                await _bookingRepository.GetBookingsByUserIdAsync(userId);

            return bookings.Select(MapToResponse);
        }

        // 6. Convert Booking entity into BookingResponse DTO
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
