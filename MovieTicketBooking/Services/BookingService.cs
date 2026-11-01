using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<BookingResponse> CreateBookingAsync(
            CreateBookingRequest request)
        {
            if (request.ShowId <= 0)
            {
                throw new ArgumentException("Invalid show ID.");
            }

            if (string.IsNullOrWhiteSpace(request.CustomerName) ||
                string.IsNullOrWhiteSpace(request.CustomerEmail) ||
                string.IsNullOrWhiteSpace(request.CustomerPhone))
            {
                throw new ArgumentException(
                    "Customer name, email and phone are required.");
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

            bool showExists =
                await _bookingRepository.ShowExistsAsync(request.ShowId);

            if (!showExists)
            {
                throw new KeyNotFoundException("Show not found.");
            }

            decimal? ticketPrice =
                await _bookingRepository.GetShowTicketPriceAsync(request.ShowId);

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
            decimal totalAmount = ticketPrice.Value * request.SeatIds.Count;

            var booking = new Booking
            {
                BookingReference = "BK" +
                    Guid.NewGuid().ToString("N")[..20].ToUpper(),

                ShowId = request.ShowId,

                CustomerName = request.CustomerName.Trim(),

                CustomerEmail = request.CustomerEmail.Trim(),

                CustomerPhone = request.CustomerPhone.Trim(),

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

            var createdBooking =
                await _bookingRepository.CreateBookingAsync(
                    booking,
                    request.SeatIds);

            return MapToResponse(createdBooking);
        }

        public async Task<BookingResponse?> GetByIdAsync(int bookingId)
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

            return MapToResponse(booking);
        }

        public async Task<BookingResponse?> CancelBookingAsync(int bookingId)
        {
            if (bookingId <= 0)
            {
                throw new ArgumentException("Invalid booking ID.");
            }

            var booking =
                await _bookingRepository.CancelBookingAsync(bookingId);

            if (booking == null)
            {
                return null;
            }

            return MapToResponse(booking);
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