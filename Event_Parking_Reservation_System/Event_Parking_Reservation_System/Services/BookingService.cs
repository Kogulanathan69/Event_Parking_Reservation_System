using Event_Parking_Reservation_System.Data;
using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Event_Parking_Reservation_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Parking_Reservation_System.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookingDto>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    EventId = b.EventId,
                    BookingDate = b.BookingDate,
                    ExpiresAt = b.ExpiresAt,
                    Status = b.Status,
                    TotalAmount = b.TotalAmount,
                    SeatIds = b.BookingSeats
                        .Select(bs => bs.SeatId)
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings
                .Where(b => b.Id == id)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    EventId = b.EventId,
                    BookingDate = b.BookingDate,
                    ExpiresAt = b.ExpiresAt,
                    Status = b.Status,
                    TotalAmount = b.TotalAmount,
                    SeatIds = b.BookingSeats
                        .Select(bs => bs.SeatId)
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<BookingDto>> GetBookingsByUserIdAsync(int userId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.BookingSeats)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                UserId = b.UserId,
                EventId = b.EventId,
                BookingDate = b.BookingDate,
                ExpiresAt = b.ExpiresAt,
                Status = b.Status,
                TotalAmount = b.TotalAmount,
                SeatIds = b.BookingSeats
                    .Select(bs => bs.SeatId)
                    .ToList()
            }).ToList();
        }

        public async Task<BookingDto> CreateBookingAsync(CreateBookingDto dto)
        {
            var alreadyBookedSeatIds = await _context.BookingSeats
                .Where(bs =>
                    dto.SeatIds.Contains(bs.SeatId) &&
                    bs.Booking != null &&
                    bs.Booking.EventId == dto.EventId &&
                    (
                        bs.Booking.Status == "Pending" ||
                        bs.Booking.Status == "Confirmed"
                    ))
                .Select(bs => bs.SeatId)
                .Distinct()
                .ToListAsync();

            if (alreadyBookedSeatIds.Any())
            {
                throw new InvalidOperationException(
                    $"Seat already booked: {string.Join(", ", alreadyBookedSeatIds)}"
                );
            }

            var now = DateTime.UtcNow;

            var booking = new Booking
            {
                UserId = dto.UserId,
                EventId = dto.EventId,
                BookingDate = now,
                ExpiresAt = now.AddMinutes(10),
                Status = "Pending",
                TotalAmount = dto.TotalAmount
            };

            foreach (var seatId in dto.SeatIds)
            {
                booking.BookingSeats.Add(new BookingSeat
                {
                    SeatId = seatId
                });
            }

            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync();

            return new BookingDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                EventId = booking.EventId,
                BookingDate = booking.BookingDate,
                ExpiresAt = booking.ExpiresAt,
                Status = booking.Status,
                TotalAmount = booking.TotalAmount,
                SeatIds = booking.BookingSeats
                    .Select(bs => bs.SeatId)
                    .ToList()
            };
        }

        public async Task<bool> CancelBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return false;
            }

            if (booking.Status == "Cancelled")
            {
                return false;
            }

            booking.Status = "Cancelled";

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ConfirmBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return false;
            }

            if (booking.Status != "Pending")
            {
                return false;
            }

            if (booking.ExpiresAt <= DateTime.UtcNow)
            {
                booking.Status = "Expired";

                await _context.SaveChangesAsync();

                return false;
            }

            booking.Status = "Confirmed";

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> ExpirePendingBookingsAsync()
        {
            var now = DateTime.UtcNow;

            var expiredBookings = await _context.Bookings
                .Where(b =>
                    b.Status == "Pending" &&
                    b.ExpiresAt <= now)
                .ToListAsync();

            foreach (var booking in expiredBookings)
            {
                booking.Status = "Expired";
            }

            await _context.SaveChangesAsync();

            return expiredBookings.Count;
        }
    }
}