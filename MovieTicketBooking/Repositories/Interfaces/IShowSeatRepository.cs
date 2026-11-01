using MovieTicketBooking.DTOs.ShowSeats;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IShowSeatRepository
    {
        Task<bool> ShowExistsAsync(int showId);

        Task<List<ShowSeatResponse>> GetByShowIdAsync(
            int showId,
            string? status);
    }
}