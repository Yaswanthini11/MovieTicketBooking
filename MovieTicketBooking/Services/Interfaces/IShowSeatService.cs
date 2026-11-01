using MovieTicketBooking.DTOs.ShowSeats;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IShowSeatService
    {
        Task<List<ShowSeatResponse>?> GetAllSeatsAsync(int showId);

        Task<List<ShowSeatResponse>?> GetAvailableSeatsAsync(int showId);

        Task<List<ShowSeatResponse>?> GetBookedSeatsAsync(int showId);
    }
}