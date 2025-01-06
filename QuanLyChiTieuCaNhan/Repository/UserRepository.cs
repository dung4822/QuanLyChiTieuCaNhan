using QuanLyChiTieuCaNhan.Access;
using QuanLyChiTieuCaNhan.Models;
using QuanLyChiTieuCaNhan.Repository.BaseRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace QuanLyChiTieuCaNhan.Repository
{
    public interface IUserRepository : IBaseCRUDRepositoy<User>
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> CheckEmailExistsAsync(string email);
        Task<bool> CheckUserNameExistsAsync(string username);
        Task<IdentityResult> AddUserAsync(User user);
        Task<string> GenerateEmailConfirmationTokenAsync(User user);
    }
    public class UserRepository : BaseCRUDRepositoy<User>, IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly UserManager<User> _userManager;

        public UserRepository(ApplicationDBContext dbContext, ILogger<UserRepository> logger, UserManager<User> userManager) : base(dbContext, logger)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public Task<IdentityResult> AddUserAsync(User user)
        {
           return  _userManager.CreateAsync(user);
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        public async Task<bool> CheckUserNameExistsAsync(string username)
        {
            return await _userManager.FindByNameAsync(username) != null;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to find user {email}");
                }
                return user;
            }catch (Exception ex)
            {
                throw new ApplicationException($"Có lỗi khi tìm bằng email:{email}.", ex);
            }
        }
        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user); 
        }

    }

}
