using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Event_Parking_Reservation_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkingController : ControllerBase
    {
        private readonly IParkingService _parkingService;

        public ParkingController(IParkingService parkingService)
        {
            _parkingService = parkingService;
        }

        #region Parking Area Endpoints

        [HttpGet("areas")]
        public async Task<IActionResult> GetAllParkingAreas()
        {
            var areas = await _parkingService.GetAllParkingAreasAsync();
            return Ok(areas);
        }

        [HttpGet("areas/{id}")]
        public async Task<IActionResult> GetParkingAreaById(int id)
        {
            var area = await _parkingService.GetParkingAreaByIdAsync(id);
            if (area == null)
            {
                return NotFound(new { message = "Parking area not found" });
            }

            return Ok(area);
        }

        [HttpPost("areas")]
        public async Task<IActionResult> CreateParkingArea([FromBody] CreateParkingAreaDto dto)
        {
            try
            {
                var area = await _parkingService.CreateParkingAreaAsync(dto);
                return CreatedAtAction(nameof(GetParkingAreaById), new { id = area.Id }, area);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("areas/{id}")]
        public async Task<IActionResult> UpdateParkingArea(int id, [FromBody] CreateParkingAreaDto dto)
        {
            try
            {
                var result = await _parkingService.UpdateParkingAreaAsync(id, dto);
                if (!result)
                {
                    return NotFound(new { message = "Parking area not found" });
                }

                return Ok(new { message = "Parking area updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("areas/{id}")]
        public async Task<IActionResult> DeleteParkingArea(int id)
        {
            var result = await _parkingService.DeleteParkingAreaAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Parking area not found" });
            }

            return Ok(new { message = "Parking area deleted or deactivated successfully" });
        }

        #endregion

        #region Parking Slot Endpoints

        [HttpGet("areas/{parkingAreaId}/slots")]
        public async Task<IActionResult> GetSlotsByParkingAreaId(int parkingAreaId)
        {
            var slots = await _parkingService.GetSlotsByParkingAreaIdAsync(parkingAreaId);
            return Ok(slots);
        }

        [HttpGet("areas/{parkingAreaId}/available-slots")]
        public async Task<IActionResult> GetAvailableSlots(int parkingAreaId, [FromQuery] int eventId)
        {
            var slots = await _parkingService.GetAvailableSlotsAsync(parkingAreaId, eventId);
            return Ok(slots);
        }

        [HttpPost("slots")]
        public async Task<IActionResult> CreateParkingSlot([FromBody] CreateParkingSlotDto dto)
        {
            try
            {
                var slot = await _parkingService.CreateParkingSlotAsync(dto);
                return Ok(slot);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("slots/{id}")]
        public async Task<IActionResult> UpdateParkingSlot(int id, [FromBody] CreateParkingSlotDto dto)
        {
            try
            {
                var result = await _parkingService.UpdateParkingSlotAsync(id, dto);
                if (!result)
                {
                    return NotFound(new { message = "Parking slot not found" });
                }

                return Ok(new { message = "Parking slot updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("slots/{id}")]
        public async Task<IActionResult> DeleteParkingSlot(int id)
        {
            var result = await _parkingService.DeleteParkingSlotAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Parking slot not found" });
            }

            return Ok(new { message = "Parking slot deleted or deactivated successfully" });
        }

        #endregion

        #region Parking Reservation Endpoints

        [HttpPost("reservations")]
        public async Task<IActionResult> CreateParkingReservation([FromBody] CreateParkingReservationDto dto)
        {
            try
            {
                var reservation = await _parkingService.CreateParkingReservationAsync(dto);
                return Ok(reservation);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("reservations/booking/{bookingId}")]
        public async Task<IActionResult> GetParkingReservationByBookingId(int bookingId)
        {
            var reservation = await _parkingService.GetParkingReservationByBookingIdAsync(bookingId);
            if (reservation == null)
            {
                return NotFound(new { message = "Parking reservation not found for this booking" });
            }

            return Ok(reservation);
        }

        [HttpPut("reservations/{id}/cancel")]
        public async Task<IActionResult> CancelParkingReservation(int id)
        {
            var result = await _parkingService.CancelParkingReservationAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Parking reservation not found or already cancelled" });
            }

            return Ok(new { message = "Parking reservation cancelled successfully" });
        }

        #endregion
    }
}
