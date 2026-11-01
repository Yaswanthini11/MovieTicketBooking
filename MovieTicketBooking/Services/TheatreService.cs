using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class TheatreService : ITheatreService
    {
        private readonly ITheatreRepository _theatreRepository;

        public TheatreService(ITheatreRepository theatreRepository)
        {
            _theatreRepository = theatreRepository;
        }

        // Create Theatre
        public async Task<TheatreResponse> AddAsync(
            CreateTheatreRequest request)
        {
            var theatre = new Theatre
            {
                Name = request.Name,
                Location = request.Location
            };

            var createdTheatre =
                await _theatreRepository.AddAsync(theatre);

            return MapToResponse(createdTheatre);
        }

        // Get All Theatres
        public async Task<List<TheatreResponse>> GetAllAsync()
        {
            var theatres =
                await _theatreRepository.GetAllAsync();

            return theatres.Select(t => MapToResponse(t)).ToList();
        }

        // Get Theatre By ID
        public async Task<TheatreResponse?> GetByIdAsync(int id)
        {
            var theatre =
                await _theatreRepository.GetByIdAsync(id);

            if (theatre == null)
            {
                return null;
            }

            return MapToResponse(theatre);
        }

        // Update Theatre
        public async Task<TheatreResponse?> UpdateAsync(
            int id,
            UpdateTheatreRequest request)
        {
            var theatre =
                await _theatreRepository.GetByIdAsync(id);

            if (theatre == null)
            {
                return null;
            }

            theatre.Name = request.Name;
            theatre.Location = request.Location;

            var updatedTheatre =
                await _theatreRepository.UpdateAsync(theatre);

            return MapToResponse(updatedTheatre);
        }

        // Delete Theatre
        public async Task<bool> DeleteAsync(int id)
        {
            return await _theatreRepository.DeleteAsync(id);
        }

        // Entity to DTO Mapping
        private static TheatreResponse MapToResponse(Theatre theatre)
        {
            return new TheatreResponse
            {
                TheatreId = theatre.TheatreId,
                Name = theatre.Name,
                Location = theatre.Location
            };
        }
    }
}