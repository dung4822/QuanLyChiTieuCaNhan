using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyChiTieuCaNhan.DTO.Category;
using QuanLyChiTieuCaNhan.Service;
using System.Security.Claims;

namespace QuanLyChiTieuCaNhan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllCategories()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy UserId từ token
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            // Gọi Service để lấy danh sách categories của user
            var categories = await _categoryService.GetAllCategoriesByUserAsync(userId);

            // Nếu không có category nào
            if (categories == null || !categories.Any())
                return NotFound("No categories found for this user.");

            return Ok(categories); // Trả về HTTP 200 với danh sách categories
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            Console.WriteLine("Request Headers:");
            foreach (var header in Request.Headers)
            {
                Console.WriteLine($"{header.Key}: {header.Value}");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy UserId từ token
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            var category = await _categoryService.CreateCategoryAsync(dto, userId);
            return CreatedAtAction(nameof(GetAllCategories), new { id = category.Id }, category);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy trực tiếp UserId từ token
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            var result = await _categoryService.UpdateCategoryAsync(id, dto);
            if (!result)
                return NotFound("Category not found or not owned by user.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy trực tiếp UserId từ token
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
                return NotFound("Category not found or not owned by user.");

            return NoContent();
        }
    }
}
