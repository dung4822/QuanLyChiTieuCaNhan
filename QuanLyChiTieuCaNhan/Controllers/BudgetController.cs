using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyChiTieuCaNhan.DTO.Budget;
using QuanLyChiTieuCaNhan.Models;
using QuanLyChiTieuCaNhan.Service;
using System.Security.Claims;

namespace QuanLyChiTieuCaNhan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBudgets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            var budgets = await _budgetService.GetBudgetsByUserAsync(userId);
            return Ok(budgets);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            var budget = await _budgetService.CreateBudgetAsync(dto, userId);
            return CreatedAtAction(nameof(GetBudgets), new { id = budget.Id }, budget);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBudget(int id, [FromBody] UpdateBudgetDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            var result = await _budgetService.UpdateBudgetAsync(id, dto, userId);
            if (!result)
                return NotFound("Budget not found or not owned by user.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            var result = await _budgetService.DeleteBudgetAsync(id, userId);
            if (!result)
                return NotFound("Budget not found or not owned by user.");

            return NoContent();
        }
    }

}
