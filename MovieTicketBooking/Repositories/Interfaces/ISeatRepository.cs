using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface ISeatRepository
    {
        Task<Seat> AddAsync(Seat seat);

        Task<List<Seat>> AddRangeAsync(List<Seat> seats);

        Task<List<Seat>> GetByScreenIdAsync(int screenId);

        Task<Seat?> GetByIdAsync(int id);

        Task<Seat> UpdateAsync(Seat seat);

        Task<bool> DeleteAsync(int id);

        Task<bool> ScreenExistsAsync(int screenId);

        Task<bool> SeatPositionExistsAsync(
            int screenId,
            string rowName,
            int seatNumber,
            int? excludeSeatId = null);
    }
}