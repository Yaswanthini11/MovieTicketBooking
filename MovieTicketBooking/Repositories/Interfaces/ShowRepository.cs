using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create show and its ShowSeats in one transaction
        public async Task<Show> AddAsync(Show show)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Get all physical seats belonging to this screen
                var seats = await _context.Seats
                    .Where(s => s.ScreenId == show.ScreenId)
                    .ToListAsync();

                if (seats.Count == 0)
                {
                    throw new InvalidOperationException(
                        "This screen does not have any physical seats.");
                }

                // Save the show first to generate ShowId
                await _context.Shows.AddAsync(show);

                await _context.SaveChangesAsync();

                // Generate one ShowSeat for every physical seat
                var showSeats = seats.Select(seat => new ShowSeat
                {
                    ShowId = show.ShowId,
                    SeatId = seat.SeatId,
                    Status = ShowSeatStatus.Available
                }).ToList();

                await _context.ShowSeats.AddRangeAsync(showSeats);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return show;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Get all shows with optional filters
        public async Task<List<Show>> GetAllAsync(
            DateTime? date,
            int? movieId,
            int? theatreId)
        {
            var query = _context.Shows.AsNoTracking().AsQueryable();

            if (date.HasValue)
            {
                var selectedDate = date.Value.Date;

                query = query.Where(s =>
                    s.StartTime >= selectedDate &&
                    s.StartTime < selectedDate.AddDays(1));
            }

            if (movieId.HasValue)
            {
                query = query.Where(s => s.MovieId == movieId.Value);
            }

            if (theatreId.HasValue)
            {
                query = query.Where(s =>
                    _context.Screens.Any(screen =>
                        screen.ScreenId == s.ScreenId &&
                        screen.TheatreId == theatreId.Value));
            }

            return await query
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        // Get show by ID
        public async Task<Show?> GetByIdAsync(int id)
        {
            return await _context.Shows
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ShowId == id);
        }

        // Get shows for a movie
        public async Task<List<Show>> GetByMovieIdAsync(int movieId)
        {
            return await _context.Shows
                .AsNoTracking()
                .Where(s => s.MovieId == movieId)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        // Get shows in a theatre
        public async Task<List<Show>> GetByTheatreIdAsync(int theatreId)
        {
            return await _context.Shows
                .AsNoTracking()
                .Where(s => _context.Screens.Any(screen =>
                    screen.ScreenId == s.ScreenId &&
                    screen.TheatreId == theatreId))
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        // Check whether movie exists
        public async Task<bool> MovieExistsAsync(int movieId)
        {
            return await _context.Movies
                .AnyAsync(m => m.MovieId == movieId);
        }

        // Check whether screen exists
        public async Task<bool> ScreenExistsAsync(int screenId)
        {
            return await _context.Screens
                .AnyAsync(s => s.ScreenId == screenId);
        }

        // Check for overlapping shows on the same screen
        public async Task<bool> HasOverlapAsync(
            int screenId,
            DateTime startTime,
            DateTime endTime,
            int? excludeShowId = null)
        {
            var query = _context.Shows.Where(s =>
                s.ScreenId == screenId &&
                startTime < s.EndTime &&
                endTime > s.StartTime);

            if (excludeShowId.HasValue)
            {
                query = query.Where(s =>
                    s.ShowId != excludeShowId.Value);
            }

            return await query.AnyAsync();
        }

        // Update show details
        public async Task<Show> UpdateAsync(Show show)
        {
            _context.Shows.Update(show);

            await _context.SaveChangesAsync();

            return show;
        }

        // Delete show and its ShowSeats
        public async Task<bool> DeleteAsync(int id)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var show = await _context.Shows
                    .FirstOrDefaultAsync(s => s.ShowId == id);

                if (show == null)
                {
                    return false;
                }

                var showSeats = await _context.ShowSeats
                    .Where(ss => ss.ShowId == id)
                    .ToListAsync();

                _context.ShowSeats.RemoveRange(showSeats);

                _context.Shows.Remove(show);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}