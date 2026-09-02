using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Event_Parking_Reservation_System.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class EventCategoryController : ControllerBase
    {
        private readonly IEventCategoryService _categoryService;

        public EventCategoryController(IEventCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService
                .GetAllCategoriesAsync();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService
                .GetCategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound("Category not found");
            }

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(
            [FromBody] CreateEventCategoryDto dto)
        {
            var category = await _categoryService
                .CreateCategoryAsync(dto);

            return CreatedAtAction(
                nameof(GetCategoryById),
                new { id = category.Id },
                category
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(
            int id,
            [FromBody] UpdateEventCategoryDto dto)
        {
            var result = await _categoryService
                .UpdateCategoryAsync(id, dto);

            if (!result)
            {
                return NotFound("Category not found");
            }

            return Ok(new
            {
                message = "Category updated successfully"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService
                    .DeleteCategoryAsync(id);

                if (!result)
                {
                    return NotFound("Category not found");
                }

                return Ok(new
                {
                    message = "Category deleted successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }
    }
}