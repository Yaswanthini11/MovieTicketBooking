using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ShowExistsAsync(int showId)
        {
            return await _context.Shows
                .AnyAsync(s => s.ShowId == showId);
        }

        public async Task<decimal?> GetShowTicketPriceAsync(int showId)
        {
            return await _context.Shows
                .Where(s => s.ShowId == showId)
                .Select(s => (decimal?)s.TicketPrice)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ShowSeat>> GetAvailableShowSeatsAsync(
            int showId,
            List<int> seatIds)
        {
            return await _context.ShowSeats
                .Where(ss =>
                    ss.ShowId == showId &&
                    seatIds.Contains(ss.SeatId) &&
                    ss.Status == ShowSeatStatus.Available)
                .ToListAsync();
        }

        public async Task<Booking> CreateBookingAsync(
            Booking booking,
            List<int> seatIds)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Recheck availability inside the transaction.
                var showSeats = await _context.ShowSeats
                    .Where(ss =>
                        ss.ShowId == booking.ShowId &&
                        seatIds.Contains(ss.SeatId))
                    .ToListAsync();

                if (showSeats.Count != seatIds.Count ||
                    showSeats.Any(ss =>
                        ss.Status != ShowSeatStatus.Available))
                {
                    throw new InvalidOperationException(
                        "One or more selected seats are no longer available.");
                }

                // Create Booking and BookingSeats.
                _context.Bookings.Add(booking);

                foreach (var showSeat in showSeats)
                {
                    showSeat.Status = ShowSeatStatus.Booked;
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return booking;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Booking?> GetByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        public async Task<Booking?> CancelBookingAsync(int bookingId)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.BookingSeats)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    return null;
                }

                if (booking.Status != BookingStatus.Confirmed)
                {
                    throw new InvalidOperationException(
                        "Only confirmed bookings can be cancelled.");
                }

                var seatIds = booking.BookingSeats
                    .Select(bs => bs.SeatId)
                    .ToList();

                var showSeats = await _context.ShowSeats
                    .Where(ss =>
                        ss.ShowId == booking.ShowId &&
                        seatIds.Contains(ss.SeatId))
                    .ToListAsync();

                if (showSeats.Count != seatIds.Count)
                {
                    throw new InvalidOperationException(
                        "The booking's show seat records could not be found.");
                }

                booking.Status = BookingStatus.Cancelled;

                foreach (var showSeat in showSeats)
                {
                    showSeat.Status = ShowSeatStatus.Available;
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return booking;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}