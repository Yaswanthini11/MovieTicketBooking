using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateBookingAsync(
            CreateBookingRequest request);

        Task<BookingResponse?> GetByIdAsync(int bookingId);

        Task<BookingResponse?> CancelBookingAsync(int bookingId);
    }
}