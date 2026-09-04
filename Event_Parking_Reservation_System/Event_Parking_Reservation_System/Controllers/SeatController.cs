using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Event_Parking_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        // GET: api/Seat
        [HttpGet]
        public async Task<IActionResult> GetAllSeats()
        {
            var seats = await _seatService.GetAllSeatsAsync();

            return Ok(seats);
        }

        // GET: api/Seat/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSeatById(int id)
        {
            var seat = await _seatService.GetSeatByIdAsync(id);

            if (seat == null)
            {
                return NotFound("Seat not found.");
            }

            return Ok(seat);
        }

        // GET: api/Seat/event/1
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetSeatsByEvent(int eventId)
        {
            var seats = await _seatService
                .GetSeatsByEventIdAsync(eventId);

            return Ok(seats);
        }

        // POST: api/Seat
        [HttpPost]
        public async Task<IActionResult> CreateSeat(
            CreateSeatDto dto)
        {
            try
            {
                var seat = await _seatService
                    .CreateSeatAsync(dto);

                return CreatedAtAction(
                    nameof(GetSeatById),
                    new { id = seat.Id },
                    seat);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Seat/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSeat(
            int id,
            UpdateSeatDto dto)
        {
            try
            {
                var updated = await _seatService
                    .UpdateSeatAsync(id, dto);

                if (!updated)
                {
                    return NotFound("Seat not found.");
                }

                return Ok("Seat updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Seat/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeat(int id)
        {
            try
            {
                var deleted = await _seatService
                    .DeleteSeatAsync(id);

                if (!deleted)
                {
                    return NotFound("Seat not found.");
                }

                return Ok("Seat deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}