using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Event_Parking_Reservation_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrivateEventBookingController : ControllerBase
    {
        private readonly IPrivateEventBookingService _service;

        public PrivateEventBookingController(
            IPrivateEventBookingService service
        )
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _service.GetAllAsync();

            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _service.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreatePrivateEventBookingDto dto
        )
        {
            var booking = await _service.CreateAsync(dto);

            return Ok(booking);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _service.CancelAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return Ok(new
            {
                message = "Private event booking cancelled"
            });
        }

        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _service.ConfirmAsync(id);

            if (!result)
            {
                return BadRequest(new
                {
                    message = "Booking cannot be confirmed"
                });
            }

            return Ok(new
            {
                message = "Private event booking confirmed"
            });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var bookings = await _service.GetByUserIdAsync(userId);

            return Ok(bookings);
        }
    }
}