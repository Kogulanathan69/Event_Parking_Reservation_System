using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Event_Parking_Reservation_System.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventService.GetAllEventsAsync();

            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventDto = await _eventService.GetEventByIdAsync(id);

            if (eventDto == null)
            {
                return NotFound("Event not found");
            }

            return Ok(eventDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(
            [FromBody] CreateEventDto dto)
        {
            try
            {
                var eventDto = await _eventService
                    .CreateEventAsync(dto);

                return CreatedAtAction(
                    nameof(GetEventById),
                    new { id = eventDto.Id },
                    eventDto
                );
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("already booked"))
                {
                    return Conflict(new
                    {
                        message = ex.Message
                    });
                }

                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            [FromBody] UpdateEventDto dto)
        {
            try
            {
                var result = await _eventService
                    .UpdateEventAsync(id, dto);

                if (!result)
                {
                    return NotFound("Event not found");
                }

                return Ok(new
                {
                    message = "Event updated successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("already booked"))
                {
                    return Conflict(new
                    {
                        message = ex.Message
                    });
                }

                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var result = await _eventService
                    .DeleteEventAsync(id);

                if (!result)
                {
                    return NotFound("Event not found");
                }

                return Ok(new
                {
                    message = "Event deleted successfully"
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