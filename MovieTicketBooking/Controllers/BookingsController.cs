
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;
using MovieTicketBooking.Constants;
using MovieTicketBooking.Authorization;

using System.Security.Claims;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IAuthorizationService _authorizationService;

        public BookingsController(
            IBookingService bookingService,
            IAuthorizationService authorizationService)
        {
            _bookingService = bookingService;
            _authorizationService = authorizationService;
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
        // Customers can view their own bookings.
        // Admins can view any booking.
        [HttpGet("{bookingId:int}")]
        [Authorize]
        public async Task<IActionResult> GetBookingById(int bookingId)
        {
            if (bookingId <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid booking ID."
                });
            }

            try
            {
                // Retrieve the actual Booking entity.
                var booking =
                    await _bookingService.GetBookingForAuthorizationAsync(
                        bookingId);

                if (booking == null)
                {
                    return NotFound(new
                    {
                        message = "Booking not found."
                    });
                }

                // Check booking ownership using custom authorization.
                var authorizationResult =
                    await _authorizationService.AuthorizeAsync(
                        User,
                        booking,
                        "BookingOwnerPolicy");

                if (!authorizationResult.Succeeded)
                {
                    return NotFound(new
                    {
                        message = "Booking not found."
                    });
                }

                // Return the BookingResponse DTO.
                var response =
                    await _bookingService.GetByIdAsync(bookingId);

                if (response == null)
                {
                    return NotFound(new
                    {
                        message = "Booking not found."
                    });
                }

                return Ok(response);
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
            if (bookingId <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid booking ID."
                });
            }

            try
            {
                // Retrieve the booking before cancellation.
                var booking =
                    await _bookingService.GetBookingForAuthorizationAsync(
                        bookingId);

                if (booking == null)
                {
                    return NotFound(new
                    {
                        message = "Booking not found."
                    });
                }

                // Authorize the current user against this booking.
                var authorizationResult =
                    await _authorizationService.AuthorizeAsync(
                        User,
                        booking,
                        "BookingOwnerPolicy");

                if (!authorizationResult.Succeeded)
                {
                    return NotFound(new
                    {
                        message = "Booking not found."
                    });
                }

                // Ownership is verified. Now perform cancellation.
                var response =
                    await _bookingService.CancelBookingAsync(bookingId);

                if (response == null)
                {
                    return NotFound(new
                    {
                        message = "Booking not found."
                    });
                }

                return Ok(response);
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
 