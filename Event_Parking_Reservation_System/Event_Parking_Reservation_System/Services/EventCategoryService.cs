using Event_Parking_Reservation_System.Data;
using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Event_Parking_Reservation_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Parking_Reservation_System.Services
{
    public class EventCategoryService : IEventCategoryService
    {
        private readonly AppDbContext _context;

        public EventCategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventCategoryDto>> GetAllCategoriesAsync()
        {
            return await _context.EventCategories
                .Select(c => new EventCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();
        }

        public async Task<EventCategoryDto?> GetCategoryByIdAsync(int id)
        {
            return await _context.EventCategories
                .Where(c => c.Id == id)
                .Select(c => new EventCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .FirstOrDefaultAsync();
        }

        public async Task<EventCategoryDto> CreateCategoryAsync(
            CreateEventCategoryDto dto)
        {
            var category = new EventCategory
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.EventCategories.Add(category);

            await _context.SaveChangesAsync();

            return new EventCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<bool> UpdateCategoryAsync(
            int id,
            UpdateEventCategoryDto dto)
        {
            var category = await _context.EventCategories.FindAsync(id);

            if (category == null)
            {
                return false;
            }

            category.Name = dto.Name;
            category.Description = dto.Description;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.EventCategories.FindAsync(id);

            if (category == null)
            {
                return false;
            }

            var isUsedByEvent = await _context.Events
                .AnyAsync(e => e.CategoryId == id);

            if (isUsedByEvent)
            {
                throw new InvalidOperationException(
                    "Category cannot be deleted because it is assigned to an event."
                );
            }

            _context.EventCategories.Remove(category);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}