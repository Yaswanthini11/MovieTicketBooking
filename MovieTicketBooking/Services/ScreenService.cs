using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class ScreenService : IScreenService
    {
        private readonly IScreenRepository _screenRepository;

        public ScreenService(IScreenRepository screenRepository)
        {
            _screenRepository = screenRepository;
        }

        // Create screen
        public async Task<ScreenResponse?> AddAsync(
            CreateScreenRequest request)
        {
            // Check theatre
            bool theatreExists =
                await _screenRepository.TheatreExistsAsync(
                    request.TheatreId);

            if (!theatreExists)
            {
                return null;
            }

            // Check duplicate screen name
            bool duplicate =
                await _screenRepository.ScreenNameExistsAsync(
                    request.TheatreId,
                    request.ScreenName);

            if (duplicate)
            {
                throw new InvalidOperationException(
                    "A screen with this name already exists in this theatre.");
            }

            var screen = new Screen
            {
                TheatreId = request.TheatreId,
                ScreenName = request.ScreenName
            };

            var createdScreen =
                await _screenRepository.AddAsync(screen);

            return MapToResponse(createdScreen);
        }

        // Get all screens
        public async Task<List<ScreenResponse>> GetAllAsync(
            int? theatreId)
        {
            var screens =
                await _screenRepository.GetAllAsync(theatreId);

            return screens.Select(MapToResponse).ToList();
        }

        // Get screen by ID
        public async Task<ScreenResponse?> GetByIdAsync(int id)
        {
            var screen =
                await _screenRepository.GetByIdAsync(id);

            if (screen == null)
            {
                return null;
            }

            return MapToResponse(screen);
        }

        // Get screens by theatre
        public async Task<List<ScreenResponse>> GetByTheatreIdAsync(
            int theatreId)
        {
            var screens =
                await _screenRepository.GetByTheatreIdAsync(
                    theatreId);

            return screens.Select(MapToResponse).ToList();
        }

        // Update screen
        public async Task<ScreenResponse?> UpdateAsync(
            int id,
            UpdateScreenRequest request)
        {
            var screen =
                await _screenRepository.GetByIdAsync(id);

            if (screen == null)
            {
                return null;
            }

            bool theatreExists =
                await _screenRepository.TheatreExistsAsync(
                    request.TheatreId);

            if (!theatreExists)
            {
                return null;
            }

            bool duplicate =
                await _screenRepository.ScreenNameExistsAsync(
                    request.TheatreId,
                    request.ScreenName,
                    id);

            if (duplicate)
            {
                throw new InvalidOperationException(
                    "A screen with this name already exists in this theatre.");
            }

            screen.TheatreId = request.TheatreId;
            screen.ScreenName = request.ScreenName;

            var updatedScreen =
                await _screenRepository.UpdateAsync(screen);

            return MapToResponse(updatedScreen);
        }

        // Delete screen
        public async Task<bool> DeleteAsync(int id)
        {
            return await _screenRepository.DeleteAsync(id);
        }

        // Entity to DTO mapping
        private static ScreenResponse MapToResponse(Screen screen)
        {
            return new ScreenResponse
            {
                ScreenId = screen.ScreenId,
                TheatreId = screen.TheatreId,
                ScreenName = screen.ScreenName
            };
        }
    }
}