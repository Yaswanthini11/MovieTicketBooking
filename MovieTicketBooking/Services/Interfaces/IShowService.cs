using MovieTicketBooking.DTOs.Shows;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IShowService
    {
        Task<ShowResponse?> AddAsync(CreateShowRequest request);

        Task<List<ShowResponse>> GetAllAsync(
            DateTime? date,
            int? movieId,
            int? theatreId);

        Task<ShowResponse?> GetByIdAsync(int id);

        Task<List<ShowResponse>> GetByMovieIdAsync(int movieId);

        Task<List<ShowResponse>> GetByTheatreIdAsync(int theatreId);

        Task<ShowResponse?> UpdateAsync(
            int id,
            UpdateShowRequest request);

        Task<bool> DeleteAsync(int id);
    }
}