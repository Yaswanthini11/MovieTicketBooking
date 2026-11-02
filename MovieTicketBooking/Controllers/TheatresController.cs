using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MovieTicketBooking.Constants;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TheatresController : ControllerBase
    {
        private readonly ITheatreService _theatreService;

        public TheatresController(ITheatreService theatreService)
        {
            _theatreService = theatreService;
        }

        // POST: api/theatres
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<TheatreResponse>> CreateTheatre(
            CreateTheatreRequest request)
        {
            var result = await _theatreService.AddAsync(request);

            return CreatedAtAction(
                nameof(GetTheatreById),
                new { id = result.TheatreId },
                result
            );
        }

        // GET: api/theatres
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<TheatreResponse>>> GetAllTheatres()
        {
            var theatres = await _theatreService.GetAllAsync();

            return Ok(theatres);
        }

        // GET: api/theatres/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TheatreResponse>> GetTheatreById(int id)
        {
            var theatre = await _theatreService.GetByIdAsync(id);

            if (theatre == null)
            {
                return NotFound("Theatre not found.");
            }

            return Ok(theatre);
        }

        // PUT: api/theatres/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<TheatreResponse>> UpdateTheatre(
            int id,
            UpdateTheatreRequest request)
        {
            var theatre = await _theatreService.UpdateAsync(id, request);

            if (theatre == null)
            {
                return NotFound("Theatre not found.");
            }

            return Ok(theatre);
        }

        // DELETE: api/theatres/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteTheatre(int id)
        {
            try
            {
                var result = await _theatreService.DeleteAsync(id);

                if (!result)
                {
                    return NotFound("Theatre not found.");
                }

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Cannot delete this theatre because it has associated screens or other records.");
            }
        }
    }
}