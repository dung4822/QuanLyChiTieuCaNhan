using Microsoft.EntityFrameworkCore;
using QuanLyChiTieuCaNhan.Access;
using QuanLyChiTieuCaNhan.Models;
using QuanLyChiTieuCaNhan.Repository.BaseRepository;

namespace QuanLyChiTieuCaNhan.Repository
{
    public interface IBudgetRepository : IBaseCRUDRepositoy<Budget>
    {
        Task<IEnumerable<Budget>> GetBudgetsByUserIdAsync(string userId);
        Task<Budget> GetBudgetWithDetailsAsync(int budgetId, string userId);
    }
    public class BudgetRepository : BaseCRUDRepositoy<Budget>, IBudgetRepository
    {
        public BudgetRepository(ApplicationDBContext dbContext, ILogger<Budget> logger)
            : base(dbContext, logger) { }

        public async Task<IEnumerable<Budget>> GetBudgetsByUserIdAsync(string userId)
        {
            return await _dbSet
                .Include(b => b.Category)
                .Where(b => b.UserId == userId && !b.IsDeleted)
                .ToListAsync();
        }

        public async Task<Budget> GetBudgetWithDetailsAsync(int budgetId, string userId)
        {
            return await _dbSet
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.BudgetId == budgetId && b.UserId == userId && !b.IsDeleted);
        }
    }
}
