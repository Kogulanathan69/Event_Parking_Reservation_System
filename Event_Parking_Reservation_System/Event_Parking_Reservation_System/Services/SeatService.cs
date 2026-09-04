using Event_Parking_Reservation_System.Data;
using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Event_Parking_Reservation_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Parking_Reservation_System.Services
{
    public class SeatService : ISeatService
    {
        private readonly AppDbContext _context;

        public SeatService(AppDbContext context)
        {
            _context = context;
        }

        // Get all seats
        public async Task<IEnumerable<SeatDto>> GetAllSeatsAsync()
        {
            return await _context.Seats
                .AsNoTracking()
                .OrderBy(s => s.EventId)
                .ThenBy(s => s.Row)
                .ThenBy(s => s.Number)
                .Select(s => new SeatDto
                {
                    Id = s.Id,
                    EventId = s.EventId,
                    Row = s.Row,
                    Number = s.Number,
                    SeatType = s.SeatType,
                    Price = s.Price
                })
                .ToListAsync();
        }

        // Get seat by id
        public async Task<SeatDto?> GetSeatByIdAsync(int id)
        {
            return await _context.Seats
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new SeatDto
                {
                    Id = s.Id,
                    EventId = s.EventId,
                    Row = s.Row,
                    Number = s.Number,
                    SeatType = s.SeatType,
                    Price = s.Price
                })
                .FirstOrDefaultAsync();
        }

        // Get seats belonging to one event
        public async Task<IEnumerable<SeatDto>> GetSeatsByEventIdAsync(int eventId)
        {
            return await _context.Seats
                .AsNoTracking()
                .Where(s => s.EventId == eventId)
                .OrderBy(s => s.Row)
                .ThenBy(s => s.Number)
                .Select(s => new SeatDto
                {
                    Id = s.Id,
                    EventId = s.EventId,
                    Row = s.Row,
                    Number = s.Number,
                    SeatType = s.SeatType,
                    Price = s.Price
                })
                .ToListAsync();
        }

        // Create seat
        public async Task<SeatDto> CreateSeatAsync(CreateSeatDto dto)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.Id == dto.EventId);

            if (!eventExists)
            {
                throw new Exception("Event not found.");
            }

            if (string.IsNullOrWhiteSpace(dto.Row))
            {
                throw new Exception("Seat row is required.");
            }

            if (dto.Number <= 0)
            {
                throw new Exception("Seat number must be greater than zero.");
            }

            if (dto.Price < 0)
            {
                throw new Exception("Seat price cannot be negative.");
            }

            var duplicateSeat = await _context.Seats
                .AnyAsync(s =>
                    s.EventId == dto.EventId &&
                    s.Row == dto.Row &&
                    s.Number == dto.Number);

            if (duplicateSeat)
            {
                throw new Exception("This seat already exists for this event.");
            }

            var seat = new Seat
            {
                EventId = dto.EventId,
                Row = dto.Row.Trim().ToUpper(),
                Number = dto.Number,
                SeatType = dto.SeatType,
                Price = dto.Price
            };

            _context.Seats.Add(seat);

            await _context.SaveChangesAsync();

            return new SeatDto
            {
                Id = seat.Id,
                EventId = seat.EventId,
                Row = seat.Row,
                Number = seat.Number,
                SeatType = seat.SeatType,
                Price = seat.Price
            };
        }

        // Update seat
        public async Task<bool> UpdateSeatAsync(int id, UpdateSeatDto dto)
        {
            var seat = await _context.Seats
                .FirstOrDefaultAsync(s => s.Id == id);

            if (seat == null)
            {
                return false;
            }

            var duplicateSeat = await _context.Seats
                .AnyAsync(s =>
                    s.Id != id &&
                    s.EventId == seat.EventId &&
                    s.Row == dto.Row &&
                    s.Number == dto.Number);

            if (duplicateSeat)
            {
                throw new Exception("This seat already exists for this event.");
            }

            seat.Row = dto.Row.Trim().ToUpper();
            seat.Number = dto.Number;
            seat.SeatType = dto.SeatType;
            seat.Price = dto.Price;

            await _context.SaveChangesAsync();

            return true;
        }

        // Delete seat
        public async Task<bool> DeleteSeatAsync(int id)
        {
            var seat = await _context.Seats
                .FirstOrDefaultAsync(s => s.Id == id);

            if (seat == null)
            {
                return false;
            }

            var isUsedInBooking = await _context.BookingSeats
                .AnyAsync(bs => bs.SeatId == id);

            if (isUsedInBooking)
            {
                throw new Exception(
                    "This seat cannot be deleted because it is already used in a booking.");
            }

            _context.Seats.Remove(seat);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}