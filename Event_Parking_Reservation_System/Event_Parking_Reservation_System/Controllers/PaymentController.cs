using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Event_Parking_Reservation_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments =
                await _paymentService.GetAllPaymentsAsync();

            return Ok(payments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment =
                await _paymentService.GetPaymentByIdAsync(id);

            if (payment == null)
            {
                return NotFound("Payment not found");
            }

            return Ok(payment);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(
            [FromBody] CreatePaymentDto dto)
        {
            try
            {
                var payment =
                    await _paymentService.CreatePaymentAsync(dto);

                return Ok(payment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetPaymentByBookingId(int bookingId)
        {
            var payment = await _paymentService
                .GetPaymentByBookingIdAsync(bookingId);

            if (payment == null)
            {
                return NotFound("Payment not found");
            }

            return Ok(payment);
        }
    }
}