using Event_Parking_Reservation_System.Data;
using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Event_Parking_Reservation_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Parking_Reservation_System.Services
{
    public class ParkingService : IParkingService
    {
        private readonly AppDbContext _context;

        public ParkingService(AppDbContext context)
        {
            _context = context;
        }

        #region Parking Area Methods

        public async Task<List<ParkingAreaDto>> GetAllParkingAreasAsync()
        {
            return await _context.ParkingAreas
                .Where(pa => pa.IsActive)
                .Select(pa => new ParkingAreaDto
                {
                    Id = pa.Id,
                    VenueId = pa.VenueId,
                    Name = pa.Name,
                    Description = pa.Description,
                    TotalSlots = pa.TotalSlots,
                    IsActive = pa.IsActive
                })
                .ToListAsync();
        }

        public async Task<ParkingAreaDto?> GetParkingAreaByIdAsync(int id)
        {
            return await _context.ParkingAreas
                .Where(pa => pa.Id == id)
                .Select(pa => new ParkingAreaDto
                {
                    Id = pa.Id,
                    VenueId = pa.VenueId,
                    Name = pa.Name,
                    Description = pa.Description,
                    TotalSlots = pa.TotalSlots,
                    IsActive = pa.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ParkingAreaDto> CreateParkingAreaAsync(CreateParkingAreaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new InvalidOperationException("Parking area name is required.");
            }

            if (dto.TotalSlots <= 0)
            {
                throw new InvalidOperationException("Total slots must be greater than zero.");
            }

            var venueExists = await _context.Venues.AnyAsync(v => v.Id == dto.VenueId);
            if (!venueExists)
            {
                throw new InvalidOperationException("Venue not found.");
            }

            var parkingArea = new ParkingArea
            {
                VenueId = dto.VenueId,
                Name = dto.Name.Trim(),
                Description = dto.Description,
                TotalSlots = dto.TotalSlots,
                IsActive = true
            };

            _context.ParkingAreas.Add(parkingArea);
            await _context.SaveChangesAsync();

            return new ParkingAreaDto
            {
                Id = parkingArea.Id,
                VenueId = parkingArea.VenueId,
                Name = parkingArea.Name,
                Description = parkingArea.Description,
                TotalSlots = parkingArea.TotalSlots,
                IsActive = parkingArea.IsActive
            };
        }

        public async Task<bool> UpdateParkingAreaAsync(int id, CreateParkingAreaDto dto)
        {
            var parkingArea = await _context.ParkingAreas.FindAsync(id);
            if (parkingArea == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new InvalidOperationException("Parking area name is required.");
            }

            if (dto.TotalSlots <= 0)
            {
                throw new InvalidOperationException("Total slots must be greater than zero.");
            }

            var venueExists = await _context.Venues.AnyAsync(v => v.Id == dto.VenueId);
            if (!venueExists)
            {
                throw new InvalidOperationException("Venue not found.");
            }

            parkingArea.VenueId = dto.VenueId;
            parkingArea.Name = dto.Name.Trim();
            parkingArea.Description = dto.Description;
            parkingArea.TotalSlots = dto.TotalSlots;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteParkingAreaAsync(int id)
        {
            var parkingArea = await _context.ParkingAreas.FindAsync(id);
            if (parkingArea == null)
            {
                return false;
            }

            var hasReservations = await _context.ParkingReservations
                .AnyAsync(pr => pr.ParkingAreaId == id);

            if (hasReservations)
            {
                // Safe behavior: deactivate instead of breaking historical reservations
                parkingArea.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }

            _context.ParkingAreas.Remove(parkingArea);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateParkingAreaAsync(int id)
        {
            var parkingArea = await _context.ParkingAreas.FindAsync(id);
            if (parkingArea == null)
            {
                return false;
            }

            parkingArea.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Parking Slot Methods

        public async Task<List<ParkingSlotDto>> GetSlotsByParkingAreaIdAsync(int parkingAreaId)
        {
            return await _context.ParkingSlots
                .Where(ps => ps.ParkingAreaId == parkingAreaId)
                .OrderBy(ps => ps.SlotNumber)
                .Select(ps => new ParkingSlotDto
                {
                    Id = ps.Id,
                    ParkingAreaId = ps.ParkingAreaId,
                    SlotNumber = ps.SlotNumber,
                    IsActive = ps.IsActive
                })
                .ToListAsync();
        }

        public async Task<List<ParkingSlotDto>> GetAvailableSlotsAsync(int parkingAreaId, int eventId)
        {
            // Find slot IDs already actively reserved for this event
            var reservedSlotIds = await _context.ParkingReservations
                .Where(pr => pr.ParkingAreaId == parkingAreaId &&
                             pr.ParkingSlotId != null &&
                             pr.Booking != null &&
                             pr.Booking.EventId == eventId &&
                             pr.Status != "Cancelled" &&
                             pr.Status != "Expired" &&
                             pr.Booking.Status != "Cancelled" &&
                             pr.Booking.Status != "Expired")
                .Select(pr => pr.ParkingSlotId!.Value)
                .Distinct()
                .ToListAsync();

            // Return active slots in the area that are not in the reserved list
            return await _context.ParkingSlots
                .Where(ps => ps.ParkingAreaId == parkingAreaId &&
                             ps.IsActive &&
                             !reservedSlotIds.Contains(ps.Id))
                .OrderBy(ps => ps.SlotNumber)
                .Select(ps => new ParkingSlotDto
                {
                    Id = ps.Id,
                    ParkingAreaId = ps.ParkingAreaId,
                    SlotNumber = ps.SlotNumber,
                    IsActive = ps.IsActive
                })
                .ToListAsync();
        }

        public async Task<ParkingSlotDto> CreateParkingSlotAsync(CreateParkingSlotDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SlotNumber))
            {
                throw new InvalidOperationException("Slot number is required.");
            }

            var parkingArea = await _context.ParkingAreas.FindAsync(dto.ParkingAreaId);
            if (parkingArea == null)
            {
                throw new InvalidOperationException("Parking area not found.");
            }

            var normalizedSlot = dto.SlotNumber.Trim();
            var duplicateExists = await _context.ParkingSlots
                .AnyAsync(ps => ps.ParkingAreaId == dto.ParkingAreaId &&
                                ps.SlotNumber == normalizedSlot);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    $"Slot number '{normalizedSlot}' already exists in this parking area."
                );
            }

            var slot = new ParkingSlot
            {
                ParkingAreaId = dto.ParkingAreaId,
                SlotNumber = normalizedSlot,
                IsActive = true
            };

            _context.ParkingSlots.Add(slot);
            await _context.SaveChangesAsync();

            return new ParkingSlotDto
            {
                Id = slot.Id,
                ParkingAreaId = slot.ParkingAreaId,
                SlotNumber = slot.SlotNumber,
                IsActive = slot.IsActive
            };
        }

        public async Task<bool> UpdateParkingSlotAsync(int id, CreateParkingSlotDto dto)
        {
            var slot = await _context.ParkingSlots.FindAsync(id);
            if (slot == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.SlotNumber))
            {
                throw new InvalidOperationException("Slot number is required.");
            }

            var parkingArea = await _context.ParkingAreas.FindAsync(dto.ParkingAreaId);
            if (parkingArea == null)
            {
                throw new InvalidOperationException("Parking area not found.");
            }

            var normalizedSlot = dto.SlotNumber.Trim();
            var duplicateExists = await _context.ParkingSlots
                .AnyAsync(ps => ps.Id != id &&
                                ps.ParkingAreaId == dto.ParkingAreaId &&
                                ps.SlotNumber == normalizedSlot);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    $"Slot number '{normalizedSlot}' already exists in this parking area."
                );
            }

            slot.ParkingAreaId = dto.ParkingAreaId;
            slot.SlotNumber = normalizedSlot;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteParkingSlotAsync(int id)
        {
            var slot = await _context.ParkingSlots.FindAsync(id);
            if (slot == null)
            {
                return false;
            }

            var hasReservations = await _context.ParkingReservations
                .AnyAsync(pr => pr.ParkingSlotId == id);

            if (hasReservations)
            {
                // Deactivate instead of deleting existing reservation history
                slot.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }

            _context.ParkingSlots.Remove(slot);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateParkingSlotAsync(int id)
        {
            var slot = await _context.ParkingSlots.FindAsync(id);
            if (slot == null)
            {
                return false;
            }

            slot.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Parking Reservation Methods

        public async Task<ParkingReservationDto> CreateParkingReservationAsync(CreateParkingReservationDto dto)
        {
            // 1. Booking exists
            var booking = await _context.Bookings.FindAsync(dto.BookingId);
            if (booking == null)
            {
                throw new InvalidOperationException("Booking not found.");
            }

            // 2. Booking does not already have an active ParkingReservation
            var existingReservation = await _context.ParkingReservations
                .AnyAsync(pr => pr.BookingId == dto.BookingId &&
                                pr.Status != "Cancelled");

            if (existingReservation)
            {
                throw new InvalidOperationException("This booking already has an active parking reservation.");
            }

            // Fetch the event associated with the booking to check Venue and ParkingFee
            var @event = await _context.Events.FindAsync(booking.EventId);
            if (@event == null)
            {
                throw new InvalidOperationException("Associated event not found for this booking.");
            }

            // 3. ParkingArea exists and is active
            var parkingArea = await _context.ParkingAreas.FindAsync(dto.ParkingAreaId);
            if (parkingArea == null || !parkingArea.IsActive)
            {
                throw new InvalidOperationException("Selected parking area not found or is inactive.");
            }

            // 4. ParkingArea must belong to the same Venue as the Booking's Event
            if (parkingArea.VenueId != @event.VenueId)
            {
                throw new InvalidOperationException("Selected parking area does not belong to the venue hosting this event.");
            }

            // 5. Validate ParkingSlot if provided
            if (dto.ParkingSlotId.HasValue)
            {
                var slot = await _context.ParkingSlots.FindAsync(dto.ParkingSlotId.Value);
                if (slot == null)
                {
                    throw new InvalidOperationException("Parking slot not found.");
                }

                if (slot.ParkingAreaId != dto.ParkingAreaId)
                {
                    throw new InvalidOperationException("Parking slot does not belong to the selected parking area.");
                }

                if (!slot.IsActive)
                {
                    throw new InvalidOperationException("Selected parking slot is currently inactive.");
                }

                var slotAlreadyReserved = await _context.ParkingReservations
                    .AnyAsync(pr => pr.ParkingSlotId == dto.ParkingSlotId.Value &&
                                    pr.Booking != null &&
                                    pr.Booking.EventId == @event.Id &&
                                    pr.Status != "Cancelled" &&
                                    pr.Status != "Expired" &&
                                    pr.Booking.Status != "Cancelled" &&
                                    pr.Booking.Status != "Expired");

                if (slotAlreadyReserved)
                {
                    throw new InvalidOperationException("Selected parking slot is already reserved for this event.");
                }
            }

            // 7 & 8. Calculate trusted ParkingFee from Event
            var parkingFee = @event.ParkingFee;

            // Update booking total amount so payment covers ticket + parking
            if (parkingFee > 0)
            {
                booking.TotalAmount += parkingFee;
            }

            // 10 & 11. If ParkingSlotId is null for EventQR style parking, generate an opaque random QrToken (no personal details)
            string? qrToken = null;
            if (!dto.ParkingSlotId.HasValue)
            {
                qrToken = $"PRK-{Guid.NewGuid():N}";
            }

            var reservation = new ParkingReservation
            {
                BookingId = dto.BookingId,
                ParkingAreaId = dto.ParkingAreaId,
                ParkingSlotId = dto.ParkingSlotId,
                VehicleNumber = dto.VehicleNumber?.Trim() ?? string.Empty,
                ParkingFee = parkingFee,
                Status = "Pending",
                QrToken = qrToken,
                ReservationDate = DateTime.UtcNow
            };

            _context.ParkingReservations.Add(reservation);
            await _context.SaveChangesAsync();

            return new ParkingReservationDto
            {
                Id = reservation.Id,
                BookingId = reservation.BookingId,
                ParkingAreaId = reservation.ParkingAreaId,
                ParkingSlotId = reservation.ParkingSlotId,
                VehicleNumber = reservation.VehicleNumber,
                ParkingFee = reservation.ParkingFee,
                Status = reservation.Status,
                QrToken = reservation.QrToken,
                ReservationDate = reservation.ReservationDate
            };
        }

        public async Task<ParkingReservationDto?> GetParkingReservationByBookingIdAsync(int bookingId)
        {
            return await _context.ParkingReservations
                .Where(pr => pr.BookingId == bookingId)
                .OrderByDescending(pr => pr.ReservationDate)
                .Select(pr => new ParkingReservationDto
                {
                    Id = pr.Id,
                    BookingId = pr.BookingId,
                    ParkingAreaId = pr.ParkingAreaId,
                    ParkingSlotId = pr.ParkingSlotId,
                    VehicleNumber = pr.VehicleNumber,
                    ParkingFee = pr.ParkingFee,
                    Status = pr.Status,
                    QrToken = pr.QrToken,
                    ReservationDate = pr.ReservationDate
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CancelParkingReservationAsync(int id)
        {
            var reservation = await _context.ParkingReservations
                .Include(pr => pr.Booking)
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (reservation == null)
            {
                return false;
            }

            if (reservation.Status == "Cancelled")
            {
                return false;
            }

            reservation.Status = "Cancelled";

            // If the booking is still pending, revert the parking fee from the booking total
            if (reservation.Booking != null &&
                reservation.Booking.Status.Equals("pending", StringComparison.OrdinalIgnoreCase) &&
                reservation.ParkingFee > 0)
            {
                reservation.Booking.TotalAmount = Math.Max(0, reservation.Booking.TotalAmount - reservation.ParkingFee);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion
    }
}
