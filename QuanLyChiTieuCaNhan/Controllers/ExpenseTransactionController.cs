using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyChiTieuCaNhan.DTO.ExpenseTransaction;
using QuanLyChiTieuCaNhan.Models;
using QuanLyChiTieuCaNhan.Service;
using System.Security.Claims;

namespace QuanLyChiTieuCaNhan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseTransactionController : ControllerBase
    {
        private readonly IExpenseTransactionService _transactionService;

        public ExpenseTransactionController(IExpenseTransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllTransactions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            var transactions = await _transactionService.GetAllTransactionsByUserAsync(userId);
            return Ok(transactions);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateExpenseTransactionDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            var (isOverBudget, overAmount, transaction) = await _transactionService.CreateTransactionAsync(dto, userId);

            if (isOverBudget)
            {
                return BadRequest(new { message = $"Vượt ngân sách! Bạn đang vượt {overAmount} VND.", isOverBudget });
            }

            return CreatedAtAction(nameof(GetAllTransactions), new { id = transaction.Id }, transaction);
        }



        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] UpdateExpenseTransactionDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            var result = await _transactionService.UpdateTransactionAsync(id, dto, userId);
            if (!result)
                return NotFound("Transaction not found or not owned by user.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            var result = await _transactionService.DeleteTransactionAsync(id, userId);
            if (!result)
                return NotFound("Transaction not found or not owned by user.");

            return NoContent();
        }
        [HttpGet("by-date")]
        [Authorize]
        public async Task<IActionResult> GetTransactionsByDateRange(DateTime startDate, DateTime endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            var transactions = await _transactionService.GetTransactionsByDateRangeAsync(userId, startDate, endDate);
            return Ok(transactions);
        }
        [HttpGet("filter")]
        [Authorize]
        public async Task<IActionResult> GetTransactionsByFilter([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            // Gọi service để lấy danh sách giao dịch theo bộ lọc
            var transactions = await _transactionService.GetTransactionsByFilterAsync(userId, startDate, endDate);

            return Ok(transactions);
        }

    }
}
