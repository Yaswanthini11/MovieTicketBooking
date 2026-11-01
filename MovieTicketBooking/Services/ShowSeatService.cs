using MovieTicketBooking.DTOs.ShowSeats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class ShowSeatService : IShowSeatService
    {
        private readonly IShowSeatRepository _showSeatRepository;

        public ShowSeatService(
            IShowSeatRepository showSeatRepository)
        {
            _showSeatRepository = showSeatRepository;
        }

        // Get complete seat layout
        public async Task<List<ShowSeatResponse>?> GetAllSeatsAsync(
            int showId)
        {
            if (!await _showSeatRepository.ShowExistsAsync(showId))
            {
                return null;
            }

            return await _showSeatRepository.GetByShowIdAsync(
                showId,
                null);
        }

        // Get only available seats
        public async Task<List<ShowSeatResponse>?> GetAvailableSeatsAsync(
            int showId)
        {
            if (!await _showSeatRepository.ShowExistsAsync(showId))
            {
                return null;
            }

            return await _showSeatRepository.GetByShowIdAsync(
                showId,
                ShowSeatStatus.Available);
        }

        // Get only booked seats
        public async Task<List<ShowSeatResponse>?> GetBookedSeatsAsync(
            int showId)
        {
            if (!await _showSeatRepository.ShowExistsAsync(showId))
            {
                return null;
            }

            return await _showSeatRepository.GetByShowIdAsync(
                showId,
                ShowSeatStatus.Booked);
        }
    }
}