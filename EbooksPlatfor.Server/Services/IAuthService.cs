using OnlineBookstore.DTOs;

namespace OnlineBookstore.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
        Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
        Task<UserDto?> GetUserProfileAsync(string userId);
        Task<UserDto> UpdateProfileAsync(string userId, UpdateProfileDto updateProfileDto);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    }
}