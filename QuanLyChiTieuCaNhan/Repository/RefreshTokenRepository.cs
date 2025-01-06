using Microsoft.EntityFrameworkCore;
using QuanLyChiTieuCaNhan.Access;
using QuanLyChiTieuCaNhan.Models;
using QuanLyChiTieuCaNhan.Repository.BaseRepository;

namespace QuanLyChiTieuCaNhan.Repository
{
    public interface IRefreshTokenRepository : IBaseCRUDRepositoy<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
       
        Task RevokeTokenAsync(string token);
    }
    public class RefreshTokenRepository : BaseCRUDRepositoy<RefreshToken>, IRefreshTokenRepository
    {

        public RefreshTokenRepository(ApplicationDBContext dbContext, ILogger<RefreshToken> logger) : base(dbContext, logger)
        {

        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token && rt.IsActive);
        }

        public async Task RevokeTokenAsync(string token)
        {
            var refreshToken = await GetByTokenAsync(token);
            if (refreshToken != null)
            {
                refreshToken.IsActive = false;
            }
        }


    }
}
