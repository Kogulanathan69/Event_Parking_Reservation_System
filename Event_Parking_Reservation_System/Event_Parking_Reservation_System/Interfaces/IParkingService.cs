using Event_Parking_Reservation_System.DTOs;

namespace Event_Parking_Reservation_System.Interfaces
{
    public interface IParkingService
    {
        // Parking Area
        Task<List<ParkingAreaDto>> GetAllParkingAreasAsync();

        Task<ParkingAreaDto?> GetParkingAreaByIdAsync(int id);

        Task<ParkingAreaDto> CreateParkingAreaAsync(CreateParkingAreaDto dto);

        Task<bool> UpdateParkingAreaAsync(int id, CreateParkingAreaDto dto);

        Task<bool> DeleteParkingAreaAsync(int id);

        Task<bool> DeactivateParkingAreaAsync(int id);

        // Parking Slot
        Task<List<ParkingSlotDto>> GetSlotsByParkingAreaIdAsync(int parkingAreaId);

        Task<List<ParkingSlotDto>> GetAvailableSlotsAsync(int parkingAreaId, int eventId);

        Task<ParkingSlotDto> CreateParkingSlotAsync(CreateParkingSlotDto dto);

        Task<bool> UpdateParkingSlotAsync(int id, CreateParkingSlotDto dto);

        Task<bool> DeleteParkingSlotAsync(int id);

        Task<bool> DeactivateParkingSlotAsync(int id);

        // Parking Reservation
        Task<ParkingReservationDto> CreateParkingReservationAsync(CreateParkingReservationDto dto);

        Task<ParkingReservationDto?> GetParkingReservationByBookingIdAsync(int bookingId);

        Task<bool> CancelParkingReservationAsync(int id);
    }
}
