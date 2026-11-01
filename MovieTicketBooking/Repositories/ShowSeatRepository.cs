using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.DTOs.ShowSeats;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class ShowSeatRepository : IShowSeatRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowSeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Check whether the show exists
        public async Task<bool> ShowExistsAsync(int showId)
        {
            return await _context.Shows
                .AnyAsync(s => s.ShowId == showId);
        }

        // Get seat layout with optional status filter
        public async Task<List<ShowSeatResponse>> GetByShowIdAsync(
            int showId,
            string? status)
        {
            var query =
                from showSeat in _context.ShowSeats
                join seat in _context.Seats
                    on showSeat.SeatId equals seat.SeatId
                join show in _context.Shows
                    on showSeat.ShowId equals show.ShowId
                where showSeat.ShowId == showId
                select new
                {
                    seat.SeatId,
                    seat.RowName,
                    seat.SeatNumber,
                    seat.SeatType,
                    show.TicketPrice,
                    showSeat.Status
                };

            // Apply status filter when provided
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }

            var result = await query
                .OrderBy(x => x.RowName)
                .ThenBy(x => x.SeatNumber)
                .ToListAsync();

            // Map query result into response DTO
            return result.Select(x => new ShowSeatResponse
            {
                SeatId = x.SeatId,
                RowName = x.RowName,
                SeatNumber = x.SeatNumber,
                SeatLabel = x.RowName + x.SeatNumber,
                SeatType = x.SeatType,
                Price = x.TicketPrice,
                Status = x.Status
            }).ToList();
        }
    }
}