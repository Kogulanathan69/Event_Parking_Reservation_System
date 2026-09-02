using Event_Parking_Reservation_System.DTOs;

namespace Event_Parking_Reservation_System.Interfaces
{
    public interface IEventService
    {
        Task<List<EventDto>> GetAllEventsAsync();

        Task<EventDto?> GetEventByIdAsync(int id);

        Task<EventDto> CreateEventAsync(CreateEventDto dto);

        Task<bool> UpdateEventAsync(int id, UpdateEventDto dto);

        Task<bool> DeleteEventAsync(int id);
    }
}