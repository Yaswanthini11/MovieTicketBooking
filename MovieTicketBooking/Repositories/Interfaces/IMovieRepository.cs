using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<Movie> AddAsync(Movie movie);

        Task<List<Movie>> GetAllAsync();

        Task<Movie?> GetByIdAsync(int id);

        Task<Movie> UpdateAsync(Movie movie);

        Task<bool> DeleteAsync(int id);

        Task<List<Movie>> SearchByTitleAsync(string title);
    }
}