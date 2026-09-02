using Event_Parking_Reservation_System.DTOs;

namespace Event_Parking_Reservation_System.Interfaces
{
    public interface IEventCategoryService
    {
        Task<List<EventCategoryDto>> GetAllCategoriesAsync();

        Task<EventCategoryDto?> GetCategoryByIdAsync(int id);

        Task<EventCategoryDto> CreateCategoryAsync(CreateEventCategoryDto dto);

        Task<bool> UpdateCategoryAsync(int id, UpdateEventCategoryDto dto);

        Task<bool> DeleteCategoryAsync(int id);
    }
}