using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.DTOs;
using OnlineBookstore.Services;
using System.Security.Claims;

namespace OnlineBookstore.Controllers
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

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResultDto>> Login(LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.LoginAsync(loginDto);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResultDto>> Register(RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.RegisterAsync(registerDto);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
            }
        }

        // POST: api/auth/refresh
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResultDto>> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshToken);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while refreshing token", error = ex.Message });
            }
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            try
            {
                await _authService.LogoutAsync(refreshToken);
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during logout", error = ex.Message });
            }
        }

        // GET: api/auth/profile
        [HttpGet("profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _authService.GetUserProfileAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving profile", error = ex.Message });
            }
        }

        // PUT: api/auth/profile
        [HttpPut("profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<UserDto>> UpdateProfile(UpdateProfileDto updateProfileDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _authService.UpdateProfileAsync(userId, updateProfileDto);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating profile", error = ex.Message });
            }
        }

        // POST: api/auth/change-password
        [HttpPost("change-password")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);
                if (!result)
                {
                    return BadRequest(new { message = "Failed to change password" });
                }

                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while changing password", error = ex.Message });
            }
        }

        // GET: api/auth/check-email
        [HttpGet("check-email")]
        public async Task<ActionResult<object>> CheckEmail([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { message = "Email parameter is required" });
                }

                var exists = await _authService.CheckEmailExistsAsync(email);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking email", error = ex.Message });
            }
        }

        // GET: api/auth/check-username
        [HttpGet("check-username")]
        public async Task<ActionResult<object>> CheckUsername([FromQuery] string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                {
                    return BadRequest(new { message = "Username parameter is required" });
                }

                var exists = await _authService.CheckUsernameExistsAsync(userName);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking username", error = ex.Message });
            }
        }
    }
}