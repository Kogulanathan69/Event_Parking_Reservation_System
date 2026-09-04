using Event_Parking_Reservation_System.DTOs;

namespace Event_Parking_Reservation_System.Interfaces
{
    public interface ISeatService
    {
        Task<IEnumerable<SeatDto>> GetAllSeatsAsync();

        Task<SeatDto?> GetSeatByIdAsync(int id);

        Task<IEnumerable<SeatDto>> GetSeatsByEventIdAsync(int eventId);

        Task<SeatDto> CreateSeatAsync(CreateSeatDto dto);

        Task<bool> UpdateSeatAsync(int id, UpdateSeatDto dto);

        Task<bool> DeleteSeatAsync(int id);
    }
}
