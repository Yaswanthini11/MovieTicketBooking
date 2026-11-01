using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create one seat
        public async Task<Seat> AddAsync(Seat seat)
        {
            await _context.Seats.AddAsync(seat);

            await _context.SaveChangesAsync();

            return seat;
        }

        // Create multiple seats
        public async Task<List<Seat>> AddRangeAsync(
            List<Seat> seats)
        {
            await _context.Seats.AddRangeAsync(seats);

            await _context.SaveChangesAsync();

            return seats;
        }

        // Get all seats belonging to a screen
        public async Task<List<Seat>> GetByScreenIdAsync(
            int screenId)
        {
            return await _context.Seats
                .Where(s => s.ScreenId == screenId)
                .OrderBy(s => s.RowName)
                .ThenBy(s => s.SeatNumber)
                .ToListAsync();
        }

        // Get seat by ID
        public async Task<Seat?> GetByIdAsync(int id)
        {
            return await _context.Seats
                .FirstOrDefaultAsync(s => s.SeatId == id);
        }

        // Update seat
        public async Task<Seat> UpdateAsync(Seat seat)
        {
            _context.Seats.Update(seat);

            await _context.SaveChangesAsync();

            return seat;
        }

        // Delete seat
        public async Task<bool> DeleteAsync(int id)
        {
            var seat = await _context.Seats.FindAsync(id);

            if (seat == null)
            {
                return false;
            }

            _context.Seats.Remove(seat);

            await _context.SaveChangesAsync();

            return true;
        }

        // Check whether screen exists
        public async Task<bool> ScreenExistsAsync(int screenId)
        {
            return await _context.Screens
                .AnyAsync(s => s.ScreenId == screenId);
        }

        // Check duplicate seat position
        public async Task<bool> SeatPositionExistsAsync(
            int screenId,
            string rowName,
            int seatNumber,
            int? excludeSeatId = null)
        {
            var query = _context.Seats.Where(s =>
                s.ScreenId == screenId &&
                s.RowName == rowName &&
                s.SeatNumber == seatNumber);

            if (excludeSeatId.HasValue)
            {
                query = query.Where(
                    s => s.SeatId != excludeSeatId.Value);
            }

            return await query.AnyAsync();
        }
    }
}