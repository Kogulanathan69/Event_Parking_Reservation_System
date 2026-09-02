using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Event_Parking_Reservation_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();

            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return NotFound("Booking not found");
            }

            return Ok(booking);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBookingsByUserId(int userId)
        {
            var bookings = await _bookingService
                .GetBookingsByUserIdAsync(userId);

            return Ok(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(
             [FromBody] CreateBookingDto createBookingDto)
        {
            try

            {
                var booking = await _bookingService
                    .CreateBookingAsync(createBookingDto);

                return CreatedAtAction(
                    nameof(GetBookingById),
                    new { id = booking.Id },
                    booking
                );
            }
                catch (InvalidOperationException ex)
            
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var result = await _bookingService
                .CancelBookingAsync(id);

            if (!result)
            {
                return NotFound("Booking not found");
            }

            return Ok("Booking cancelled successfully");
        }

        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmBooking(int id)
        {
            var result = await _bookingService.ConfirmBookingAsync(id);

            if (!result)
            {
                return NotFound("Booking not found");
            }

            return Ok("Booking confirmed successfully");
        }

        [HttpPut("expire-pending")]
        public async Task<IActionResult> ExpirePendingBookings()
        {
            var count = await _bookingService.ExpirePendingBookingsAsync();

            return Ok(new
            {
                message = $"{count} booking(s) expired"
            });
        }
    }
}