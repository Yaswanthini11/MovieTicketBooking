using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;

        public SeatService(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        // Create one seat
        public async Task<SeatResponse?> AddAsync(
            CreateSeatRequest request)
        {
            bool screenExists =
                await _seatRepository.ScreenExistsAsync(
                    request.ScreenId);

            if (!screenExists)
            {
                return null;
            }

            string rowName = request.RowName.Trim().ToUpperInvariant();

            bool duplicate =
                await _seatRepository.SeatPositionExistsAsync(
                    request.ScreenId,
                    rowName,
                    request.SeatNumber);

            if (duplicate)
            {
                throw new InvalidOperationException(
                    "A seat already exists at this position in the screen.");
            }

            var seat = new Seat
            {
                ScreenId = request.ScreenId,
                RowName = rowName,
                SeatNumber = request.SeatNumber,
                SeatType = request.SeatType.Trim()
            };

            var createdSeat =
                await _seatRepository.AddAsync(seat);

            return MapToResponse(createdSeat);
        }

        // Bulk create seats
        public async Task<List<SeatResponse>?> AddBulkAsync(
            BulkCreateSeatsRequest request)
        {
            bool screenExists =
                await _seatRepository.ScreenExistsAsync(
                    request.ScreenId);

            if (!screenExists)
            {
                return null;
            }

            var normalizedSeats = request.Seats
                .Select(s => new
                {
                    RowName = s.RowName.Trim().ToUpperInvariant(),
                    s.SeatNumber,
                    SeatType = s.SeatType.Trim()
                })
                .ToList();

            // Check duplicate positions inside the same request
            bool duplicateInRequest = normalizedSeats
                .GroupBy(s => new
                {
                    s.RowName,
                    s.SeatNumber
                })
                .Any(g => g.Count() > 1);

            if (duplicateInRequest)
            {
                throw new InvalidOperationException(
                    "The bulk request contains duplicate seat positions.");
            }

            // Check whether any position already exists in database
            foreach (var item in normalizedSeats)
            {
                bool duplicate =
                    await _seatRepository.SeatPositionExistsAsync(
                        request.ScreenId,
                        item.RowName,
                        item.SeatNumber);

                if (duplicate)
                {
                    throw new InvalidOperationException(
                        $"Seat {item.RowName}{item.SeatNumber} already exists in this screen.");
                }
            }

            var seats = normalizedSeats.Select(item => new Seat
            {
                ScreenId = request.ScreenId,
                RowName = item.RowName,
                SeatNumber = item.SeatNumber,
                SeatType = item.SeatType
            }).ToList();

            var createdSeats =
                await _seatRepository.AddRangeAsync(seats);

            return createdSeats.Select(MapToResponse).ToList();
        }

        // Get seats by screen
        public async Task<List<SeatResponse>?> GetByScreenIdAsync(
            int screenId)
        {
            bool screenExists =
                await _seatRepository.ScreenExistsAsync(screenId);

            if (!screenExists)
            {
                return null;
            }

            var seats =
                await _seatRepository.GetByScreenIdAsync(screenId);

            return seats.Select(MapToResponse).ToList();
        }

        // Get seat by ID
        public async Task<SeatResponse?> GetByIdAsync(int id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);

            if (seat == null)
            {
                return null;
            }

            return MapToResponse(seat);
        }

        // Update seat
        public async Task<SeatResponse?> UpdateAsync(
            int id,
            UpdateSeatRequest request)
        {
            var seat = await _seatRepository.GetByIdAsync(id);

            if (seat == null)
            {
                return null;
            }

            bool screenExists =
                await _seatRepository.ScreenExistsAsync(
                    request.ScreenId);

            if (!screenExists)
            {
                return null;
            }

            string rowName = request.RowName.Trim().ToUpperInvariant();

            bool duplicate =
                await _seatRepository.SeatPositionExistsAsync(
                    request.ScreenId,
                    rowName,
                    request.SeatNumber,
                    id);

            if (duplicate)
            {
                throw new InvalidOperationException(
                    "A seat already exists at this position in the screen.");
            }

            seat.ScreenId = request.ScreenId;
            seat.RowName = rowName;
            seat.SeatNumber = request.SeatNumber;
            seat.SeatType = request.SeatType.Trim();

            var updatedSeat =
                await _seatRepository.UpdateAsync(seat);

            return MapToResponse(updatedSeat);
        }

        // Delete seat
        public async Task<bool> DeleteAsync(int id)
        {
            return await _seatRepository.DeleteAsync(id);
        }

        // Entity to DTO mapping
        private static SeatResponse MapToResponse(Seat seat)
        {
            return new SeatResponse
            {
                SeatId = seat.SeatId,
                ScreenId = seat.ScreenId,
                RowName = seat.RowName,
                SeatNumber = seat.SeatNumber,
                SeatType = seat.SeatType
            };
        }
    }
}