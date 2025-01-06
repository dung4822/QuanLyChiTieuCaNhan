using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using QuanLyChiTieuCaNhan.DTO.Auth;
using QuanLyChiTieuCaNhan.Service;
using System.Security.Claims;

namespace QuanLyChiTieuCaNhan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
        {
            /*  if (!ModelState.IsValid)
                  return BadRequest(ModelState);

              var result = await _authService.RegisterAsync(model);
              if (!result.Succeeded)
                  return StatusCode(500, result.Errors);

              return Ok("User registered successfully.");*/
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterAsync(model);
            if (!result.Succeeded)
            {
                return StatusCode(500, result.Errors);
            }
            return Ok("User register successfully.");

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _authService.LoginAsync(model);
            return Ok(new { Token = token });
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest("Invalid confirmation request.");

            try
            {
                var result = await _authService.ConfirmEmailAsync(userId, token);
                if (result)
                    return Ok("Email confirmed successfully.");
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest("Something went wrong.");
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto model)
        {
            try
            {
                var tokenResult = await _authService.RefreshTokenAsync(model.RefreshToken);
                Response.Cookies.Append("refreshToken", tokenResult.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                return Ok(new { AccessToken = tokenResult.AccessToken });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Error = ex.Message });
            }
        }
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            var user = await _authService.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            return Ok(user); // `user` đã là DTO
        }
        [HttpGet("test")]
        public async Task<IActionResult> test()
        {
            return Ok(new { Success = true });
        }
    }

}
