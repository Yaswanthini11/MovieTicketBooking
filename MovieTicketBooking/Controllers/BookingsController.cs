using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // POST: api/bookings
        [HttpPost]
        public async Task<IActionResult> CreateBooking(
            CreateBookingRequest request)
        {
            try
            {
                var booking =
                    await _bookingService.CreateBookingAsync(request);

                return CreatedAtAction(
                    nameof(GetBookingById),
                    new { bookingId = booking.BookingId },
                    booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "The booking could not be completed due to a database conflict."
                });
            }
        }

        // GET: api/bookings/1
        [HttpGet("{bookingId:int}")]
        public async Task<IActionResult> GetBookingById(int bookingId)
        {
            try
            {
                var booking =
                    await _bookingService.GetByIdAsync(bookingId);

                if (booking == null)
                {
                    return NotFound(new
                    {
                        message = "Booking not found."
                    });
                }

                return Ok(booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/bookings/1/cancel
        [HttpPut("{bookingId:int}/cancel")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            try
            {
                var booking =
                    await _bookingService.CancelBookingAsync(bookingId);

                if (booking == null)
                {
                    return NotFound(new
                    {
                        message = "Booking not found."
                    });
                }

                return Ok(booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "The booking could not be cancelled due to a database conflict."
                });
            }
        }
    }
}