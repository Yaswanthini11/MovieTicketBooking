using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IScreenRepository
    {
        Task<Screen> AddAsync(Screen screen);

        Task<List<Screen>> GetAllAsync(int? theatreId);

        Task<Screen?> GetByIdAsync(int id);

        Task<List<Screen>> GetByTheatreIdAsync(int theatreId);

        Task<Screen> UpdateAsync(Screen screen);

        Task<bool> DeleteAsync(int id);

        Task<bool> TheatreExistsAsync(int theatreId);

        Task<bool> ScreenNameExistsAsync(
            int theatreId,
            string screenName,
            int? excludeScreenId = null);
    }
}