using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScreensController : ControllerBase
    {
        private readonly IScreenService _screenService;

        public ScreensController(IScreenService screenService)
        {
            _screenService = screenService;
        }

        // POST: api/screens
        [HttpPost]
        public async Task<ActionResult<ScreenResponse>> CreateScreen(
            CreateScreenRequest request)
        {
            try
            {
                var result =
                    await _screenService.AddAsync(request);

                if (result == null)
                {
                    return NotFound("Theatre not found.");
                }

                return CreatedAtAction(
                    nameof(GetScreenById),
                    new { id = result.ScreenId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Unable to create screen. Check the theatre and screen details.");
            }
        }

        // GET: api/screens
        // GET: api/screens?theatreId=1
        [HttpGet]
        public async Task<ActionResult<List<ScreenResponse>>> GetAllScreens(
            [FromQuery] int? theatreId)
        {
            var screens =
                await _screenService.GetAllAsync(theatreId);

            return Ok(screens);
        }

        // GET: api/screens/1
        [HttpGet("{id}")]
        public async Task<ActionResult<ScreenResponse>> GetScreenById(
            int id)
        {
            var screen =
                await _screenService.GetByIdAsync(id);

            if (screen == null)
            {
                return NotFound("Screen not found.");
            }

            return Ok(screen);
        }

        // GET: api/screens/theatre/1
        [HttpGet("theatre/{theatreId}")]
        public async Task<ActionResult<List<ScreenResponse>>>
            GetScreensByTheatre(int theatreId)
        {
            var screens =
                await _screenService.GetByTheatreIdAsync(theatreId);

            return Ok(screens);
        }

        // PUT: api/screens/1
        [HttpPut("{id}")]
        public async Task<ActionResult<ScreenResponse>> UpdateScreen(
            int id,
            UpdateScreenRequest request)
        {
            try
            {
                var screen =
                    await _screenService.UpdateAsync(id, request);

                if (screen == null)
                {
                    return NotFound(
                        "Screen or theatre not found.");
                }

                return Ok(screen);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Unable to update screen. Check the screen details.");
            }
        }

        // DELETE: api/screens/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScreen(int id)
        {
            try
            {
                bool result =
                    await _screenService.DeleteAsync(id);

                if (!result)
                {
                    return NotFound("Screen not found.");
                }

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Cannot delete this screen because it has associated seats or other records.");
            }
        }
    }
}