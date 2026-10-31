using MovieTicketBooking.DTOs.Movies;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IMovieService
    {
        Task<MovieResponse> AddAsync(CreateMovieRequest request);

        Task<List<MovieResponse>> GetAllAsync();

        Task<MovieResponse?> GetByIdAsync(int id);

        Task<MovieResponse?> UpdateAsync(int id, UpdateMovieRequest request);

        Task<List<MovieResponse>> SearchByTitleAsync(string title);

        Task<bool> DeleteAsync(int id);
    }
}