using Event_Parking_Reservation_System.Data;
using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Event_Parking_Reservation_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Parking_Reservation_System.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly IBookingService _bookingService;

        public PaymentService(
            AppDbContext context,
            IBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        public async Task<List<PaymentDto>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    BookingId = p.BookingId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    Status = p.Status,
                    PaymentDate = p.PaymentDate,
                    TransactionReference = p.TransactionReference
                })
                .ToListAsync();
        }

        public async Task<PaymentDto?> GetPaymentByIdAsync(int id)
        {
            return await _context.Payments
                .Where(p => p.Id == id)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    BookingId = p.BookingId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    Status = p.Status,
                    PaymentDate = p.PaymentDate,
                    TransactionReference = p.TransactionReference
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PaymentDto> CreatePaymentAsync(
            CreatePaymentDto dto)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == dto.BookingId);

            if (booking == null)
            {
                throw new InvalidOperationException(
                    "Booking not found"
                );
            }

            if (booking.Status != "Pending")
            {
                throw new InvalidOperationException(
                    $"Payment cannot be made for booking with status {booking.Status}"
                );
            }

            if (booking.ExpiresAt <= DateTime.UtcNow)
            {
                booking.Status = "Expired";

                await _context.SaveChangesAsync();

                throw new InvalidOperationException(
                    "Booking has expired"
                );
            }

            if (dto.Amount != booking.TotalAmount)
            {
                throw new InvalidOperationException(
                    "Payment amount does not match booking total"
                );
            }

            var existingPayment = await _context.Payments
                .AnyAsync(p =>
                    p.BookingId == dto.BookingId &&
                    p.Status == "Success");

            if (existingPayment)
            {
                throw new InvalidOperationException(
                    "Booking has already been paid"
                );
            }

            var payment = new Payment
            {
                BookingId = dto.BookingId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                Status = "Success",
                PaymentDate = DateTime.UtcNow,

                TransactionReference =
                    $"PAY-{Guid.NewGuid().ToString()[..8].ToUpper()}"
            };

            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            var confirmed = await _bookingService
                .ConfirmBookingAsync(dto.BookingId);

            if (!confirmed)
            {
                throw new InvalidOperationException(
                    "Unable to confirm booking"
                );
            }

            return new PaymentDto
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                PaymentDate = payment.PaymentDate,
                TransactionReference = payment.TransactionReference
            };
        }

        public async Task<PaymentDto?> GetPaymentByBookingIdAsync(int bookingId)
        {
            return await _context.Payments
                .Where(p => p.BookingId == bookingId)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    BookingId = p.BookingId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    Status = p.Status,
                    PaymentDate = p.PaymentDate,
                    TransactionReference = p.TransactionReference
                })
                .FirstOrDefaultAsync();
        }
    }
}