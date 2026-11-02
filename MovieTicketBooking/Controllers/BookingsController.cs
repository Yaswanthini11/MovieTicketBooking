
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;
using MovieTicketBooking.Constants;
using System.Security.Claims;

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
        // Only Customers can create bookings.
        [HttpPost]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<IActionResult> CreateBooking(
            CreateBookingRequest request)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var booking =
                    await _bookingService.CreateBookingAsync(
                        request, userId);

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

        // GET: api/bookings/my-bookings
        // Customers can view only their own bookings.
        [HttpGet("my-bookings")]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var bookings =
                await _bookingService.GetBookingsByUserIdAsync(userId);

            return Ok(bookings);
        }

        // GET: api/bookings/1
        // Customers can view their own booking.
        // Admins can view any booking.
        [HttpGet("{bookingId:int}")]
        [Authorize]
        public async Task<IActionResult> GetBookingById(int bookingId)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var isAdmin = User.IsInRole(UserRoles.Admin);

            try
            {
                var booking =
                    await _bookingService.GetByIdAsync(
                        bookingId, userId, isAdmin);

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
        // Customers can cancel their own bookings.
        // Admins can cancel any booking.
        [HttpPut("{bookingId:int}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var isAdmin = User.IsInRole(UserRoles.Admin);

            try
            {
                var booking =
                    await _bookingService.CancelBookingAsync(
                        bookingId, userId, isAdmin);

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
