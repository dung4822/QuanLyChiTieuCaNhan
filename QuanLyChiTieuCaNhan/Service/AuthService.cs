using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using QuanLyChiTieuCaNhan.CustomExceptions.ResourceExceptions;
using QuanLyChiTieuCaNhan.DTO.Auth;
using QuanLyChiTieuCaNhan.Models;
using QuanLyChiTieuCaNhan.Repository;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyChiTieuCaNhan.Service
{
    public interface IAuthService
    {
        public string GenerateJWTTokenAsync(string userId, string name, string email);
        Task<IdentityResult> RegisterAsync(RegisterUserDto registerUserDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<AuthResponseDto> RefreshTokenAsync(string token);
        Task<UserResponseDto> FindByIdAsync(string id);
    }
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(IConfiguration configuration, ILogger<AuthService> logger, IUserRepository userRepository, IMapper mapper, UserManager<User> userManager, IEmailService emailService, IRefreshTokenRepository refreshTokenRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
            _userManager = userManager;
            _emailService = emailService;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public string GenerateJWTTokenAsync(string userId, string name, string email)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, userId), // ID người dùng
        new Claim(ClaimTypes.Name, name),              // Tên người dùng
        new Claim(ClaimTypes.Email, email),            // Email người dùng
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique Identifier
        new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtConfig:Issuer"],
                audience: _configuration["JwtConfig:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15), // Thời gian hết hạn
                signingCredentials: creds
            );

            var jwttoken = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"Generated Token: {jwttoken}");
            return jwttoken;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                throw new UnauthorizedAccessException("Invalid credentials.");

            var accessToken = GenerateJWTTokenAsync(user.Id, user.UserName, user.Email);
            var refreshToken = GenerateRefreshToken();

            var newRefreshToken = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                Expiration = DateTime.UtcNow.AddDays(7),

            };

            await _refreshTokenRepository.AddAsync(newRefreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<IdentityResult> RegisterAsync(RegisterUserDto registerUserDto)
        {
            // Kiểm tra email đã tồn tại
            var emailExists = await _userRepository.CheckEmailExistsAsync(registerUserDto.Email);
            if (emailExists)
                throw new ApplicationException("Email already exists.");

            // Map dữ liệu từ DTO sang User
            var user = _mapper.Map<User>(registerUserDto);
            user.EmailConfirmed = false; // Đặt mặc định chưa xác nhận email

            // Tạo người dùng
            var result = await _userManager.CreateAsync(user, registerUserDto.Password);
            if (!result.Succeeded)
                throw new ApplicationException("Failed to register user.");

            // Tạo token xác nhận email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Link xác nhận email
            var confirmationLink = $"{_configuration["FrontendUrl"]}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            // Gửi email xác nhận
            await _emailService.SendEmailAsync(
                registerUserDto.Email,
                "Confirm your email",
                $"Please confirm your email by clicking <a href=\"{confirmationLink}\">here</a>."
            );

            return result;
        }
        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ApplicationException("User not found.");
            var decodetoken = Uri.UnescapeDataString(token);
            var result = await _userManager.ConfirmEmailAsync(user, decodetoken);
            if (!result.Succeeded)
                throw new ApplicationException("Email confirmation failed.");

            return true;
        }
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }


        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (storedToken == null || storedToken.Expiration < DateTime.UtcNow || !storedToken.IsActive)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            storedToken.IsActive = false; // Thu hồi refresh token cũ
            await _refreshTokenRepository.SaveChangesAsync();

            var newRefreshToken = GenerateRefreshToken();
            var user = await _userManager.FindByIdAsync(storedToken.UserId);

            if (user == null)
                throw new ApplicationException("User not found.");

            var newAccessToken = GenerateJWTTokenAsync(user.Id, user.UserName, user.Email);

            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                Token = newRefreshToken,
                UserId = storedToken.UserId,
                Expiration = DateTime.UtcNow.AddDays(7)
            });

            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<UserResponseDto> FindByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new ApplicationException("User not exist");

            var responseUser = _mapper.Map<UserResponseDto>(user);
            return responseUser;
        }
    }
}
