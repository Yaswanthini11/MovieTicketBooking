using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<bool> ShowExistsAsync(int showId);

        Task<decimal?> GetShowTicketPriceAsync(int showId);

        Task<List<ShowSeat>> GetAvailableShowSeatsAsync(
            int showId,
            List<int> seatIds);

        Task<Booking> CreateBookingAsync(
            Booking booking,
            List<int> seatIds);

        Task<Booking?> GetByIdAsync(int bookingId);

        Task<Booking?> CancelBookingAsync(int bookingId);

        // Get all bookings belonging to a particular customer.
        Task<List<Booking>> GetBookingsByUserIdAsync(string userId);
    }
}