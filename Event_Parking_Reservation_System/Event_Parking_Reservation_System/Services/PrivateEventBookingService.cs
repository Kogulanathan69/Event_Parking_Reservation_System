using Event_Parking_Reservation_System.Data;
using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Event_Parking_Reservation_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Parking_Reservation_System.Services
{
    public class PrivateEventBookingService
        : IPrivateEventBookingService
    {
        private readonly AppDbContext _context;

        public PrivateEventBookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PrivateEventBooking>> GetAllAsync()
        {
            return await _context.PrivateEventBookings
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<PrivateEventBooking?> GetByIdAsync(int id)
        {
            return await _context.PrivateEventBookings
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PrivateEventBooking> CreateAsync(
            CreatePrivateEventBookingDto dto
        )
        {
            var booking = new PrivateEventBooking
            {
                UserId = dto.UserId,
                EventType = dto.EventType,
                EventName = dto.EventName,
                VenueId = dto.VenueId,
                EventDate = dto.EventDate,
                GuestCount = dto.GuestCount,
                NeedParking = dto.NeedParking,
                ParkingAreaId = dto.ParkingAreaId,
                TotalAmount = dto.TotalAmount,
                Status = "Pending"
            };

            _context.PrivateEventBookings.Add(booking);

            await _context.SaveChangesAsync();

            return booking;
        }

        public async Task<bool> CancelAsync(int id)
        {
            var booking = await _context.PrivateEventBookings
                .FirstOrDefaultAsync(x => x.Id == id);

            if (booking == null)
            {
                return false;
            }

            if (booking.Status != "Pending")
            {
                return false;
            }

            booking.Status = "Cancelled";

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ConfirmAsync(int id)
        {
            var booking = await _context.PrivateEventBookings
                .FirstOrDefaultAsync(x => x.Id == id);

            if (booking == null)
            {
                return false;
            }

            if (booking.Status != "Pending")
            {
                return false;
            }

            booking.Status = "Confirmed";

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<PrivateEventBooking>> GetByUserIdAsync(int userId)
        {
            return await _context.PrivateEventBookings
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }
    }
}