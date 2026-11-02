using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateBookingAsync(
            CreateBookingRequest request,
            string userId);

        Task<BookingResponse?> GetByIdAsync(
            int bookingId,
            string userId,
            bool isAdmin);

        Task<BookingResponse?> CancelBookingAsync(
            int bookingId,
            string userId,
            bool isAdmin);

        Task<IEnumerable<BookingResponse>> GetBookingsByUserIdAsync(
            string userId);
    }
}