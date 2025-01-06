using Microsoft.EntityFrameworkCore;
using QuanLyChiTieuCaNhan.Access;
using QuanLyChiTieuCaNhan.Models;
using QuanLyChiTieuCaNhan.Repository.BaseRepository;

namespace QuanLyChiTieuCaNhan.Repository
{
    public interface IExpenseTransactionRepository : IBaseCRUDRepositoy<ExpenseTransaction>
    {
        Task<IEnumerable<ExpenseTransaction>> GetTransactionsByUserIdAsync(string userId);
        Task<ExpenseTransaction> GetTransactionWithCategoryAsync(int transactionId);
        Task<IEnumerable<ExpenseTransaction>> GetTransactionsByBudgetIdAsync(int budgetId, string userId);
        Task<Budget> GetRelatedBudgetAsync(int categoryId, DateTime transactionDate, string userId); // Kiểm tra ngân sách
        Task<decimal> GetActualSpendingAsync(int categoryId, DateTime startDate, DateTime endDate, string userId); // Lấy chi tiêu thực tế
        Task<IEnumerable<ExpenseTransaction>> GetTransactionsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate);

        Task<IEnumerable<ExpenseTransaction>> GetTransactionsByFilterAsync(string userId, DateTime? startDate, DateTime? endDate);


    }
    public class ExpenseTransactionRepository : BaseCRUDRepositoy<ExpenseTransaction>, IExpenseTransactionRepository
    {
        public ExpenseTransactionRepository(ApplicationDBContext dbContext, ILogger<ExpenseTransaction> logger)
            : base(dbContext, logger) { }

        public async Task<IEnumerable<ExpenseTransaction>> GetTransactionsByUserIdAsync(string userId)
        {
            return await _dbSet
                .Include(e => e.Category) // Include Category để lấy tên category
                .Where(e => e.UserId == userId && !e.IsDelete)
                .ToListAsync();
        }
        public async Task<ExpenseTransaction> GetTransactionWithCategoryAsync(int transactionId)
        {
            return await _dbSet
                .Include(e => e.Category) // Include để lấy thông tin Category
                .FirstOrDefaultAsync(e => e.ExpenseTransactionId == transactionId && !e.IsDelete);
        }
        public async Task<IEnumerable<ExpenseTransaction>> GetTransactionsByBudgetIdAsync(int budgetId, string userId)
        {
            // Lấy ngân sách hiện tại
            var budget = await _dbContext.Budgets
                .FirstOrDefaultAsync(b => b.BudgetId == budgetId && b.UserId == userId && !b.IsDeleted);

            if (budget == null)
                return Enumerable.Empty<ExpenseTransaction>(); // Không có ngân sách, trả về danh sách rỗng

            // Lấy giao dịch thuộc ngân sách
            return await _dbSet
                .Include(et => et.Category) // Bao gồm thông tin Category
                .Where(et => et.UserId == userId &&
                             et.CategoryId == budget.CategoryId && // Lọc theo CategoryId
                             !et.IsDelete && // Không lấy giao dịch bị xóa
                             et.TransactionDate >= budget.StartDate && // Trong khoảng thời gian ngân sách
                             et.TransactionDate <= budget.EndDate)
                .ToListAsync();
        }
        public async Task<Budget> GetRelatedBudgetAsync(int categoryId, DateTime transactionDate, string userId)
        {
            return await _dbContext.Budgets
                .Where(b => b.UserId == userId &&
                            b.CategoryId == categoryId &&
                            b.StartDate <= transactionDate &&
                            b.EndDate >= transactionDate &&
                            !b.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<decimal> GetActualSpendingAsync(int categoryId, DateTime startDate, DateTime endDate, string userId)
        {
            return await _dbSet
                .Where(et => et.CategoryId == categoryId &&
                             et.TransactionDate >= startDate &&
                             et.TransactionDate <= endDate &&
                             et.UserId == userId &&
                             !et.IsDelete)
                .SumAsync(et => et.Amount);
        }


        public async Task<IEnumerable<ExpenseTransaction>> GetTransactionsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(e => e.Category) // Bao gồm thông tin danh mục
                .Where(e => e.UserId == userId &&
                            !e.IsDelete &&
                            e.TransactionDate >= startDate &&
                            e.TransactionDate <= endDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExpenseTransaction>> GetTransactionsByFilterAsync(string userId, DateTime? startDate, DateTime? endDate)
        {
            var query = _dbSet.AsQueryable();

            // Lọc theo user
            query = query.Where(t => t.UserId == userId && !t.IsDelete);

            // Lọc theo ngày bắt đầu (nếu có)
            if (startDate.HasValue)
                query = query.Where(t => t.TransactionDate >= startDate.Value);

            // Lọc theo ngày kết thúc (nếu có)
            if (endDate.HasValue)
                query = query.Where(t => t.TransactionDate <= endDate.Value);

            return await query.Include(t => t.Category).ToListAsync();
        }

    }
}
