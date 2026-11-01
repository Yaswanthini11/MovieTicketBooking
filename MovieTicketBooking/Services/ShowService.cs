using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class ShowService : IShowService
    {
        private readonly IShowRepository _showRepository;

        public ShowService(IShowRepository showRepository)
        {
            _showRepository = showRepository;
        }

        // Create a show
        public async Task<ShowResponse?> AddAsync(
            CreateShowRequest request)
        {
            if (!await _showRepository.MovieExistsAsync(request.MovieId))
            {
                throw new InvalidOperationException(
                    "Movie not found.");
            }

            if (!await _showRepository.ScreenExistsAsync(request.ScreenId))
            {
                throw new InvalidOperationException(
                    "Screen not found.");
            }

            ValidateShowTime(request.StartTime, request.EndTime);

            if (request.TicketPrice < 0)
            {
                throw new InvalidOperationException(
                    "Ticket price cannot be negative.");
            }

            bool hasOverlap = await _showRepository.HasOverlapAsync(
                request.ScreenId,
                request.StartTime,
                request.EndTime);

            if (hasOverlap)
            {
                throw new InvalidOperationException(
                    "This screen already has a show during the selected time.");
            }

            var show = new Show
            {
                MovieId = request.MovieId,
                ScreenId = request.ScreenId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TicketPrice = request.TicketPrice
            };

            var createdShow = await _showRepository.AddAsync(show);

            return MapToResponse(createdShow);
        }

        // Get all shows with filters
        public async Task<List<ShowResponse>> GetAllAsync(
            DateTime? date,
            int? movieId,
            int? theatreId)
        {
            var shows = await _showRepository.GetAllAsync(
                date,
                movieId,
                theatreId);

            return shows.Select(MapToResponse).ToList();
        }

        // Get show by ID
        public async Task<ShowResponse?> GetByIdAsync(int id)
        {
            var show = await _showRepository.GetByIdAsync(id);

            if (show == null)
            {
                return null;
            }

            return MapToResponse(show);
        }

        // Get shows for a movie
        public async Task<List<ShowResponse>> GetByMovieIdAsync(
            int movieId)
        {
            var shows = await _showRepository.GetByMovieIdAsync(movieId);

            return shows.Select(MapToResponse).ToList();
        }

        // Get shows in a theatre
        public async Task<List<ShowResponse>> GetByTheatreIdAsync(
            int theatreId)
        {
            var shows = await _showRepository.GetByTheatreIdAsync(theatreId);

            return shows.Select(MapToResponse).ToList();
        }

        // Update show
        public async Task<ShowResponse?> UpdateAsync(
            int id,
            UpdateShowRequest request)
        {
            var existingShow = await _showRepository.GetByIdAsync(id);

            if (existingShow == null)
            {
                return null;
            }

            if (!await _showRepository.MovieExistsAsync(request.MovieId))
            {
                throw new InvalidOperationException(
                    "Movie not found.");
            }

            ValidateShowTime(request.StartTime, request.EndTime);

            if (request.TicketPrice < 0)
            {
                throw new InvalidOperationException(
                    "Ticket price cannot be negative.");
            }

            bool hasOverlap = await _showRepository.HasOverlapAsync(
                existingShow.ScreenId,
                request.StartTime,
                request.EndTime,
                id);

            if (hasOverlap)
            {
                throw new InvalidOperationException(
                    "This screen already has a show during the selected time.");
            }

            existingShow.MovieId = request.MovieId;
            existingShow.StartTime = request.StartTime;
            existingShow.EndTime = request.EndTime;
            existingShow.TicketPrice = request.TicketPrice;

            var updatedShow =
                await _showRepository.UpdateAsync(existingShow);

            return MapToResponse(updatedShow);
        }

        // Delete show
        public async Task<bool> DeleteAsync(int id)
        {
            return await _showRepository.DeleteAsync(id);
        }

        // Validate show timings
        private static void ValidateShowTime(
            DateTime startTime,
            DateTime endTime)
        {
            if (endTime <= startTime)
            {
                throw new InvalidOperationException(
                    "End time must be later than start time.");
            }
        }

        // Convert Show model into response DTO
        private static ShowResponse MapToResponse(Show show)
        {
            return new ShowResponse
            {
                ShowId = show.ShowId,
                MovieId = show.MovieId,
                ScreenId = show.ScreenId,
                StartTime = show.StartTime,
                EndTime = show.EndTime,
                TicketPrice = show.TicketPrice
            };
        }
    }
}