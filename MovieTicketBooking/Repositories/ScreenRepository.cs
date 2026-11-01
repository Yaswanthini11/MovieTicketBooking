using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class ScreenRepository : IScreenRepository
    {
        private readonly ApplicationDbContext _context;

        public ScreenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create screen
        public async Task<Screen> AddAsync(Screen screen)
        {
            await _context.Screens.AddAsync(screen);

            await _context.SaveChangesAsync();

            return screen;
        }

        // Get all screens with optional theatre filter
        public async Task<List<Screen>> GetAllAsync(int? theatreId)
        {
            var query = _context.Screens.AsQueryable();

            if (theatreId.HasValue)
            {
                query = query.Where(
                    s => s.TheatreId == theatreId.Value);
            }

            return await query
                .OrderBy(s => s.ScreenId)
                .ToListAsync();
        }

        // Get screen by ID
        public async Task<Screen?> GetByIdAsync(int id)
        {
            return await _context.Screens
                .FirstOrDefaultAsync(s => s.ScreenId == id);
        }

        // Get screens belonging to a theatre
        public async Task<List<Screen>> GetByTheatreIdAsync(
            int theatreId)
        {
            return await _context.Screens
                .Where(s => s.TheatreId == theatreId)
                .OrderBy(s => s.ScreenId)
                .ToListAsync();
        }

        // Update screen
        public async Task<Screen> UpdateAsync(Screen screen)
        {
            _context.Screens.Update(screen);

            await _context.SaveChangesAsync();

            return screen;
        }

        // Delete screen
        public async Task<bool> DeleteAsync(int id)
        {
            var screen = await _context.Screens.FindAsync(id);

            if (screen == null)
            {
                return false;
            }

            _context.Screens.Remove(screen);

            await _context.SaveChangesAsync();

            return true;
        }

        // Check whether theatre exists
        public async Task<bool> TheatreExistsAsync(int theatreId)
        {
            return await _context.Theatres
                .AnyAsync(t => t.TheatreId == theatreId);
        }

        // Check duplicate screen name within a theatre
        public async Task<bool> ScreenNameExistsAsync(
            int theatreId,
            string screenName,
            int? excludeScreenId = null)
        {
            var query = _context.Screens.Where(s =>
                s.TheatreId == theatreId &&
                s.ScreenName == screenName);

            if (excludeScreenId.HasValue)
            {
                query = query.Where(
                    s => s.ScreenId != excludeScreenId.Value);
            }

            return await query.AnyAsync();
        }
    }
}