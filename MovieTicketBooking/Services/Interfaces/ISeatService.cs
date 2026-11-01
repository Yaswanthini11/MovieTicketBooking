using MovieTicketBooking.DTOs.Seats;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface ISeatService
    {
        Task<SeatResponse?> AddAsync(CreateSeatRequest request);

        Task<List<SeatResponse>?> AddBulkAsync(
            BulkCreateSeatsRequest request);

        Task<List<SeatResponse>?> GetByScreenIdAsync(int screenId);

        Task<SeatResponse?> GetByIdAsync(int id);

        Task<SeatResponse?> UpdateAsync(
            int id,
            UpdateSeatRequest request);

        Task<bool> DeleteAsync(int id);
    }
}