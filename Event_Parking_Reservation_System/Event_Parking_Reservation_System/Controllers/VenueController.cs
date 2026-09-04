using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Event_Parking_Reservation_System.Controllers
{
    [ApiController]
    [Route("api/venues")]
    public class VenueController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenueController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVenues()
        {
            var venues = await _venueService.GetAllVenuesAsync();

            return Ok(venues);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVenueById(int id)
        {
            var venue = await _venueService.GetVenueByIdAsync(id);

            if (venue == null)
            {
                return NotFound("Venue not found");
            }

            return Ok(venue);
        }

        [HttpGet("available")]
        public async Task<IActionResult> CheckVenueAvailability(
            [FromQuery] int venueId,
            [FromQuery] DateTime startDateTime,
            [FromQuery] DateTime endDateTime)
        {
            if (startDateTime >= endDateTime)
            {
                return BadRequest(new
                {
                    message = "End time must be after start time."
                });
            }

            var isAvailable = await _venueService
                .IsVenueAvailableAsync(
                    venueId,
                    startDateTime,
                    endDateTime
                );

            return Ok(new
            {
                venueId = venueId,
                available = isAvailable
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateVenue(
            [FromBody] CreateVenueDto dto)
        {
            var venue = await _venueService.CreateVenueAsync(dto);

            return CreatedAtAction(
                nameof(GetVenueById),
                new { id = venue.Id },
                venue
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVenue(
            int id,
            [FromBody] UpdateVenueDto dto)
        {
            var result = await _venueService
                .UpdateVenueAsync(id, dto);

            if (!result)
            {
                return NotFound("Venue not found");
            }

            return Ok(new
            {
                message = "Venue updated successfully"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenue(int id)
        {
            try
            {
                var result = await _venueService
                    .DeleteVenueAsync(id);

                if (!result)
                {
                    return NotFound("Venue not found");
                }

                return Ok(new
                {
                    message = "Venue deleted successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }
    }
}