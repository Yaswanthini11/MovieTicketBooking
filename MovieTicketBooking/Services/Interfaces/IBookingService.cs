
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateBookingAsync(
            CreateBookingRequest request,
            string userId);

        Task<Booking?> GetBookingForAuthorizationAsync(
            int bookingId);

        Task<BookingResponse?> GetByIdAsync(
            int bookingId);

        Task<BookingResponse?> CancelBookingAsync(
            int bookingId);

        Task<IEnumerable<BookingResponse>> GetBookingsByUserIdAsync(
            string userId);
    }
}
