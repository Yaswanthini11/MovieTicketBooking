using MovieTicketBooking.DTOs.Screens;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IScreenService
    {
        Task<ScreenResponse?> AddAsync(CreateScreenRequest request);

        Task<List<ScreenResponse>> GetAllAsync(int? theatreId);

        Task<ScreenResponse?> GetByIdAsync(int id);

        Task<List<ScreenResponse>> GetByTheatreIdAsync(int theatreId);

        Task<ScreenResponse?> UpdateAsync(
            int id,
            UpdateScreenRequest request);

        Task<bool> DeleteAsync(int id);
    }
}