using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatsController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        // POST: api/seats
        [HttpPost]
        public async Task<ActionResult<SeatResponse>> CreateSeat(
            CreateSeatRequest request)
        {
            try
            {
                var result =
                    await _seatService.AddAsync(request);

                if (result == null)
                {
                    return NotFound("Screen not found.");
                }

                return CreatedAtAction(
                    nameof(GetSeatById),
                    new { id = result.SeatId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Unable to create seat. Check the seat details.");
            }
        }

        // POST: api/seats/bulk
        [HttpPost("bulk")]
        public async Task<ActionResult<List<SeatResponse>>>
            CreateSeatsBulk(BulkCreateSeatsRequest request)
        {
            try
            {
                var result =
                    await _seatService.AddBulkAsync(request);

                if (result == null)
                {
                    return NotFound("Screen not found.");
                }

                return StatusCode(
                    StatusCodes.Status201Created,
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Unable to create seats. Check for duplicate seat positions.");
            }
        }

        // GET: api/seats/screen/1
        [HttpGet("screen/{screenId}")]
        public async Task<ActionResult<List<SeatResponse>>>
            GetSeatsByScreen(int screenId)
        {
            var seats =
                await _seatService.GetByScreenIdAsync(screenId);

            if (seats == null)
            {
                return NotFound("Screen not found.");
            }

            return Ok(seats);
        }

        // GET: api/seats/1
        [HttpGet("{id}")]
        public async Task<ActionResult<SeatResponse>> GetSeatById(
            int id)
        {
            var seat = await _seatService.GetByIdAsync(id);

            if (seat == null)
            {
                return NotFound("Seat not found.");
            }

            return Ok(seat);
        }

        // PUT: api/seats/1
        [HttpPut("{id}")]
        public async Task<ActionResult<SeatResponse>> UpdateSeat(
            int id,
            UpdateSeatRequest request)
        {
            try
            {
                var seat =
                    await _seatService.UpdateAsync(id, request);

                if (seat == null)
                {
                    return NotFound("Seat or screen not found.");
                }

                return Ok(seat);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Unable to update seat. Check the seat details.");
            }
        }

        // DELETE: api/seats/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeat(int id)
        {
            try
            {
                bool result =
                    await _seatService.DeleteAsync(id);

                if (!result)
                {
                    return NotFound("Seat not found.");
                }

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    "Cannot delete this seat because it has associated show or booking records.");
            }
        }
    }
}