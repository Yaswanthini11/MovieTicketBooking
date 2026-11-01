using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IShowRepository
    {
        Task<Show> AddAsync(Show show);

        Task<List<Show>> GetAllAsync(
            DateTime? date,
            int? movieId,
            int? theatreId);

        Task<Show?> GetByIdAsync(int id);

        Task<List<Show>> GetByMovieIdAsync(int movieId);

        Task<List<Show>> GetByTheatreIdAsync(int theatreId);

        Task<bool> MovieExistsAsync(int movieId);

        Task<bool> ScreenExistsAsync(int screenId);

        Task<bool> HasOverlapAsync(
            int screenId,
            DateTime startTime,
            DateTime endTime,
            int? excludeShowId = null);

        Task<Show> UpdateAsync(Show show);

        Task<bool> DeleteAsync(int id);
    }
}