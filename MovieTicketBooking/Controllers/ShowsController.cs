using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MovieTicketBooking.Constants;


namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly IShowService _showService;
        private readonly IShowSeatService _showSeatService;

        public ShowsController(
     IShowService showService,
     IShowSeatService showSeatService)
        {
            _showService = showService;
            _showSeatService = showSeatService;
        }

        // POST: api/shows
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> CreateShow(
            CreateShowRequest request)
        {
            try
            {
                var result = await _showService.AddAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result!.ShowId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Unable to schedule this show due to a database constraint."
                });
            }
        }

        // GET: api/shows?date=2026-11-01&movieId=1&theatreId=1
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(
            [FromQuery] DateTime? date,
            [FromQuery] int? movieId,
            [FromQuery] int? theatreId)
        {
            if (movieId.HasValue && movieId.Value <= 0)
            {
                return BadRequest(new { message = "Invalid movieId." });
            }

            if (theatreId.HasValue && theatreId.Value <= 0)
            {
                return BadRequest(new { message = "Invalid theatreId." });
            }

            var result = await _showService.GetAllAsync(
                date,
                movieId,
                theatreId);

            return Ok(result);
        }

        // GET: api/shows/1
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _showService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = "Show not found." });
            }

            return Ok(result);
        }

        // GET: api/shows/movie/1
        [HttpGet("movie/{movieId:int}")]
        [AllowAnonymous]

        public async Task<IActionResult> GetByMovie(int movieId)
        {
            if (movieId <= 0)
            {
                return BadRequest(new { message = "Invalid movieId." });
            }

            var result = await _showService.GetByMovieIdAsync(movieId);

            return Ok(result);
        }

        // GET: api/shows/theatre/1
        [HttpGet("theatre/{theatreId:int}")]

        [AllowAnonymous]
        public async Task<IActionResult> GetByTheatre(int theatreId)
        {
            if (theatreId <= 0)
            {
                return BadRequest(new { message = "Invalid theatreId." });
            }

            var result = await _showService.GetByTheatreIdAsync(theatreId);

            return Ok(result);
        }

        // GET: api/shows/search?date=2026-11-01
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchByDate(
            [FromQuery] DateTime date)
        {
            var result = await _showService.GetAllAsync(
                date,
                null,
                null);

            return Ok(result);
        }

        // PUT: api/shows/1
        [HttpPut("{id:int}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UpdateShow(
            int id,
            UpdateShowRequest request)
        {
            try
            {
                var result = await _showService.UpdateAsync(id, request);

                if (result == null)
                {
                    return NotFound(new { message = "Show not found." });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Unable to update the show due to a database constraint."
                });
            }
        }

        // DELETE: api/shows/1
        [HttpDelete("{id:int}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteShow(int id)
        {
            try
            {
                bool deleted = await _showService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new { message = "Show not found." });
                }

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "This show cannot be deleted because it has dependent booking records."
                });
            }
        }
        // GET: api/shows/1/seats
        [HttpGet("{showId:int}/seats")]
        [AllowAnonymous]
        public async Task<IActionResult> GetShowSeats(int showId)
        {
            if (showId <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid show ID."
                });
            }

            var result = await _showSeatService.GetAllSeatsAsync(showId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Show not found."
                });
            }

            return Ok(result);
        }


        // GET: api/shows/1/seats/available
        [HttpGet("{showId:int}/seats/available")]
        [AllowAnonymous]

        public async Task<IActionResult> GetAvailableSeats(int showId)
        {
            if (showId <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid show ID."
                });
            }

            var result =
                await _showSeatService.GetAvailableSeatsAsync(showId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Show not found."
                });
            }

            return Ok(result);
        }


        // GET: api/shows/1/seats/booked
        [HttpGet("{showId:int}/seats/booked")]

        [AllowAnonymous]
        public async Task<IActionResult> GetBookedSeats(int showId)
        {
            if (showId <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid show ID."
                });
            }

            var result =
                await _showSeatService.GetBookedSeatsAsync(showId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Show not found."
                });
            }

            return Ok(result);
        }
    }
}