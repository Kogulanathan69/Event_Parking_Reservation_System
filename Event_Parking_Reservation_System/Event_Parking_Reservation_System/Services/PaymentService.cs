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
                .OrderByDescending(p => p.PaymentDate)
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

        public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto)
        {
            // 1. Booking Validation
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == dto.BookingId);

            if (booking == null)
            {
                throw new InvalidOperationException("Booking not found.");
            }

            if (booking.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Payment cannot be processed for a cancelled booking.");
            }

            if (booking.Status.Equals("Expired", StringComparison.OrdinalIgnoreCase) || booking.ExpiresAt <= DateTime.UtcNow)
            {
                booking.Status = "Expired";
                await _context.SaveChangesAsync();
                throw new InvalidOperationException("Booking has expired.");
            }

            if (!booking.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Payment cannot be processed for booking with status '{booking.Status}'.");
            }

            // 2. Validate Payment Method (Only Card, Cash, BankTransfer allowed)
            var validMethods = new[] { "Card", "Cash", "BankTransfer" };
            var matchedMethod = validMethods.FirstOrDefault(m => m.Equals(dto.PaymentMethod?.Trim(), StringComparison.OrdinalIgnoreCase));

            if (matchedMethod == null)
            {
                throw new InvalidOperationException("Invalid payment method. Allowed methods are: Card, Cash, BankTransfer.");
            }

            // 3. Prevent duplicate successful payments
            var alreadyPaid = await HasSuccessfulPaymentAsync(dto.BookingId);
            if (alreadyPaid)
            {
                throw new InvalidOperationException("Booking has already been paid successfully.");
            }

            // 4. Create Simulated Payment (Backend trusted amount from Booking.TotalAmount)
            var payment = new Payment
            {
                BookingId = dto.BookingId,
                Amount = booking.TotalAmount,
                PaymentMethod = matchedMethod,
                Status = "Success",
                PaymentDate = DateTime.UtcNow,
                TransactionReference = $"PAY-{Guid.NewGuid().ToString()[..8].ToUpper()}"
            };

            _context.Payments.Add(payment);

            // 5. Confirm Booking
            booking.Status = "Confirmed";

            await _context.SaveChangesAsync();

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
            // Preference 1: Successful payment if available
            var successfulPayment = await _context.Payments
                .Where(p => p.BookingId == bookingId && p.Status == "Success")
                .OrderByDescending(p => p.PaymentDate)
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

            if (successfulPayment != null)
            {
                return successfulPayment;
            }

            // Preference 2: Otherwise latest payment attempt
            return await _context.Payments
                .Where(p => p.BookingId == bookingId)
                .OrderByDescending(p => p.PaymentDate)
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

        public async Task<List<PaymentDto>> GetPaymentsByBookingIdAsync(int bookingId)
        {
            return await _context.Payments
                .Where(p => p.BookingId == bookingId)
                .OrderByDescending(p => p.PaymentDate)
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

        public async Task<bool> HasSuccessfulPaymentAsync(int bookingId)
        {
            return await _context.Payments
                .AnyAsync(p => p.BookingId == bookingId && p.Status == "Success");
        }
    }
}