using Microsoft.EntityFrameworkCore;
using QuanLyChiTieuCaNhan.Access;
using QuanLyChiTieuCaNhan.Models;
using QuanLyChiTieuCaNhan.Repository.BaseRepository;

namespace QuanLyChiTieuCaNhan.Repository
{
    public interface ICategoryRepository : IBaseCRUDRepositoy<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesByUserIdAsync(string userId);
    }
    public class CategoryRepository : BaseCRUDRepositoy<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDBContext dbContext, ILogger<Category> logger) : base(dbContext, logger) { }

        public async Task<IEnumerable<Category>> GetCategoriesByUserIdAsync(string userId)
        {
            return await _dbSet.Where(c => c.UserId == userId && !c.IsDelete).ToListAsync();
        }
    }
}
