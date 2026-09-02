using Event_Parking_Reservation_System.Data;
using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Event_Parking_Reservation_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Parking_Reservation_System.Services
{
    public class EventService : IEventService
    {
        private readonly AppDbContext _context;

        public EventService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventDto>> GetAllEventsAsync()
        {
            return await _context.Events
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    VenueId = e.VenueId,
                    VenueName = e.Venue != null ? e.Venue.Name : string.Empty,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category != null ? e.Category.Name : string.Empty,
                    StartDateTime = e.StartDateTime,
                    EndDateTime = e.EndDateTime,
                    TicketPrice = e.TicketPrice,
                    ParkingFee = e.ParkingFee,
                    Capacity = e.Capacity
                })
                .ToListAsync();
        }

        public async Task<EventDto?> GetEventByIdAsync(int id)
        {
            return await _context.Events
                .Where(e => e.Id == id)
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    VenueId = e.VenueId,
                    VenueName = e.Venue != null ? e.Venue.Name : string.Empty,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category != null ? e.Category.Name : string.Empty,
                    StartDateTime = e.StartDateTime,
                    EndDateTime = e.EndDateTime,
                    TicketPrice = e.TicketPrice,
                    ParkingFee = e.ParkingFee,
                    Capacity = e.Capacity
                })
                .FirstOrDefaultAsync();
        }

        public async Task<EventDto> CreateEventAsync(CreateEventDto dto)
        {
            if (dto.StartDateTime >= dto.EndDateTime)
            {
                throw new InvalidOperationException(
                    "Event end time must be after start time."
                );
            }

            var venue = await _context.Venues.FindAsync(dto.VenueId);

            if (venue == null)
            {
                throw new InvalidOperationException("Venue not found.");
            }

            var category = await _context.EventCategories
                .FindAsync(dto.CategoryId);

            if (category == null)
            {
                throw new InvalidOperationException("Category not found.");
            }

            if (dto.Capacity > venue.Capacity)
            {
                throw new InvalidOperationException(
                    "Event capacity cannot exceed venue capacity."
                );
            }

            var hasOverlap = await _context.Events
                .AnyAsync(e =>
                    e.VenueId == dto.VenueId &&
                    dto.StartDateTime < e.EndDateTime &&
                    dto.EndDateTime > e.StartDateTime);

            if (hasOverlap)
            {
                throw new InvalidOperationException(
                    "Venue is already booked for this date and time."
                );
            }

            var eventEntity = new Event
            {
                Name = dto.Name,
                VenueId = dto.VenueId,
                CategoryId = dto.CategoryId,
                StartDateTime = dto.StartDateTime,
                EndDateTime = dto.EndDateTime,
                TicketPrice = dto.TicketPrice,
                ParkingFee = dto.ParkingFee,
                Capacity = dto.Capacity
            };

            _context.Events.Add(eventEntity);

            await _context.SaveChangesAsync();

            return new EventDto
            {
                Id = eventEntity.Id,
                Name = eventEntity.Name,
                VenueId = eventEntity.VenueId,
                VenueName = venue.Name,
                CategoryId = eventEntity.CategoryId,
                CategoryName = category.Name,
                StartDateTime = eventEntity.StartDateTime,
                EndDateTime = eventEntity.EndDateTime,
                TicketPrice = eventEntity.TicketPrice,
                ParkingFee = eventEntity.ParkingFee,
                Capacity = eventEntity.Capacity
            };
        }

        public async Task<bool> UpdateEventAsync(
            int id,
            UpdateEventDto dto)
        {
            var eventEntity = await _context.Events.FindAsync(id);

            if (eventEntity == null)
            {
                return false;
            }

            if (dto.StartDateTime >= dto.EndDateTime)
            {
                throw new InvalidOperationException(
                    "Event end time must be after start time."
                );
            }

            var venue = await _context.Venues.FindAsync(dto.VenueId);

            if (venue == null)
            {
                throw new InvalidOperationException("Venue not found.");
            }

            var category = await _context.EventCategories
                .FindAsync(dto.CategoryId);

            if (category == null)
            {
                throw new InvalidOperationException("Category not found.");
            }

            if (dto.Capacity > venue.Capacity)
            {
                throw new InvalidOperationException(
                    "Event capacity cannot exceed venue capacity."
                );
            }

            var bookedSeatCount = await _context.BookingSeats
                .CountAsync(bs =>
                    bs.Booking != null &&
                    bs.Booking.EventId == id &&
                    (
                        bs.Booking.Status == "Pending" ||
                        bs.Booking.Status == "Confirmed"
                    ));

            if (dto.Capacity < bookedSeatCount)
            {
                throw new InvalidOperationException(
                    "Event capacity cannot be lower than booked seat count."
                );
            }

            var hasOverlap = await _context.Events
                .AnyAsync(e =>
                    e.Id != id &&
                    e.VenueId == dto.VenueId &&
                    dto.StartDateTime < e.EndDateTime &&
                    dto.EndDateTime > e.StartDateTime);

            if (hasOverlap)
            {
                throw new InvalidOperationException(
                    "Venue is already booked for this date and time."
                );
            }

            eventEntity.Name = dto.Name;
            eventEntity.VenueId = dto.VenueId;
            eventEntity.CategoryId = dto.CategoryId;
            eventEntity.StartDateTime = dto.StartDateTime;
            eventEntity.EndDateTime = dto.EndDateTime;
            eventEntity.TicketPrice = dto.TicketPrice;
            eventEntity.ParkingFee = dto.ParkingFee;
            eventEntity.Capacity = dto.Capacity;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var eventEntity = await _context.Events.FindAsync(id);

            if (eventEntity == null)
            {
                return false;
            }

            var hasActiveBookings = await _context.Bookings
                .AnyAsync(b =>
                    b.EventId == id &&
                    (
                        b.Status == "Pending" ||
                        b.Status == "Confirmed"
                    ));

            if (hasActiveBookings)
            {
                throw new InvalidOperationException(
                    "Event cannot be deleted because it has active bookings."
                );
            }

            _context.Events.Remove(eventEntity);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}