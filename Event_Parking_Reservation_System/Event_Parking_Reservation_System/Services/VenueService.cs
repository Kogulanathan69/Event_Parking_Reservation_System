using Event_Parking_Reservation_System.Data;
using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Event_Parking_Reservation_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Parking_Reservation_System.Services
{
    public class VenueService : IVenueService
    {
        private readonly AppDbContext _context;

        public VenueService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<VenueDto>> GetAllVenuesAsync()
        {
            return await _context.Venues
                .Select(v => new VenueDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Address = v.Address,
                    Capacity = v.Capacity
                })
                .ToListAsync();
        }

        public async Task<VenueDto?> GetVenueByIdAsync(int id)
        {
            return await _context.Venues
                .Where(v => v.Id == id)
                .Select(v => new VenueDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Address = v.Address,
                    Capacity = v.Capacity
                })
                .FirstOrDefaultAsync();
        }

        public async Task<VenueDto> CreateVenueAsync(CreateVenueDto dto)
        {
            var venue = new Venue
            {
                Name = dto.Name,
                Address = dto.Address,
                Capacity = dto.Capacity
            };

            _context.Venues.Add(venue);

            await _context.SaveChangesAsync();

            return new VenueDto
            {
                Id = venue.Id,
                Name = venue.Name,
                Address = venue.Address,
                Capacity = venue.Capacity
            };
        }

        public async Task<bool> UpdateVenueAsync(
            int id,
            UpdateVenueDto dto)
        {
            var venue = await _context.Venues.FindAsync(id);

            if (venue == null)
            {
                return false;
            }

            venue.Name = dto.Name;
            venue.Address = dto.Address;
            venue.Capacity = dto.Capacity;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteVenueAsync(int id)
        {
            var venue = await _context.Venues.FindAsync(id);

            if (venue == null)
            {
                return false;
            }

            var hasUpcomingEvents = await _context.Events
                .AnyAsync(e =>
                    e.VenueId == id &&
                    e.EndDateTime > DateTime.UtcNow);

            if (hasUpcomingEvents)
            {
                throw new InvalidOperationException(
                    "Venue cannot be deleted because it has upcoming events."
                );
            }

            _context.Venues.Remove(venue);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsVenueAvailableAsync(
            int venueId,
            DateTime startDateTime,
            DateTime endDateTime)
        {
            var hasOverlap = await _context.Events
                .AnyAsync(e =>
                    e.VenueId == venueId &&
                    startDateTime < e.EndDateTime &&
                    endDateTime > e.StartDateTime);

            return !hasOverlap;
        }
    }
}