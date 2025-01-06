using Microsoft.EntityFrameworkCore;
using QuanLyChiTieuCaNhan.Access;

namespace QuanLyChiTieuCaNhan.Repository.BaseRepository
{
    public interface IBaseCRUDRepositoy<T> where T : class 
    {
        Task<IEnumerable<T>> GetAllAsync ();
        Task<T> GetByIdAsync (int id);
        Task<T> GetByIdAsync(string id);
        Task<T> AddAsync(T entity);
        Task<bool> UpdateAsync (T entity);
        Task<bool> DeleteAsync (int id);
        Task<bool> DeleteAsync(string id);
        Task SaveChangesAsync();
    }
    public class BaseCRUDRepositoy<T> : IBaseCRUDRepositoy<T> where T : class
    {
        protected readonly ApplicationDBContext _dbContext;
        protected readonly DbSet<T> _dbSet;
        protected readonly ILogger _logger;
        public BaseCRUDRepositoy(ApplicationDBContext dbContext,ILogger logger) 
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
            _logger = logger;
        }

        public async Task<T> AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return entity;
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi thêm entity có ID: {Id}", entity.ToString());
                throw new ApplicationException($"An error occurred while Adding entity with ID: {entity}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity == null)
                    return false;

                _dbSet.Remove(entity);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi xóa entity có ID: {Id}", id);
                throw new ApplicationException($"An error occurred while deleting entity with ID: {id}", ex);
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity == null)
                    return false;

                _dbSet.Remove(entity);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi xóa entity có ID: {Id}", id);
                throw new ApplicationException($"An error occurred while deleting entity with ID: {id}", ex);
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try {
               return await _dbSet.ToListAsync();
            }
            catch(Exception ex) 
            {
                throw new ApplicationException("An error occurred while retrieving data",ex);
            }
        }

        public async Task<T> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }catch(Exception ex) 
            {
                _logger.LogError(ex, "Lỗi xảy ra khi lấy entity có ID: {Id}", id);
                throw new ApplicationException($"Lỗi khi lấy entity có ID: {id}", ex);
            }
        }

        public async Task<T> GetByIdAsync(string id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi lấy entity có ID: {Id}", id);
                throw new ApplicationException($"Lỗi khi lấy entity có ID: {id}", ex);
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                return await _dbContext.SaveChangesAsync() > 0;
            }catch(Exception ex)
            {
                _logger.LogError($"Có lỗi xảy ra khi sửa {typeof(T)} có dữ liệu là {entity.ToString}");
                throw new ApplicationException("An error occurred while updating the entity.", ex);
            }
        }
        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
