using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MovieTicketBooking.Constants;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _service;

        public MoviesController(IMovieService service)
        {
            _service = service;
        }

        // POST: api/movies
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> CreateMovie(
            CreateMovieRequest request)
        {
            var result = await _service.AddAsync(request);

            return CreatedAtAction(
                nameof(GetMovieById),
                new { id = result.MovieId },
                result);
        }

        // GET: api/movies
        [HttpGet]
        [AllowAnonymous]
       
        public async Task<IActionResult> GetAllMovies()
        {
            var result = await _service.GetAllAsync();

            return Ok(result);
        }

        // GET: api/movies/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound("Movie not found.");

            return Ok(result);
        }

        // PUT: api/movies/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(
            int id, UpdateMovieRequest request)
        {
            var result = await _service.UpdateAsync(id, request);

            if (result == null)
                return NotFound("Movie not found.");

            return Ok(result);
        }

        // DELETE: api/movies/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);

                if (!result)
                    return NotFound("Movie not found.");

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "This movie cannot be deleted because it has dependent records.");
            }
        }

        // GET: api/movies/search?title=leo
        [HttpGet("search")]
        public async Task<IActionResult> SearchMovies(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("Search title is required.");

            var result = await _service.SearchByTitleAsync(title);

            return Ok(result);
        }
    }
}