using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface ITheatreRepository
    {
        Task<Theatre> AddAsync(Theatre theatre);

        Task<List<Theatre>> GetAllAsync();

        Task<Theatre?> GetByIdAsync(int id);

        Task<Theatre> UpdateAsync(Theatre theatre);

        Task<bool> DeleteAsync(int id);
    }
}