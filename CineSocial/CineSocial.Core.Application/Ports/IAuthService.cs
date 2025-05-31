using CineSocial.Core.Application.DTOs.Auth;
using CineSocial.Core.Application.DTOs.Common;

namespace CineSocial.Core.Application.Ports;

/// <summary>
/// Primary Port - Authentication Service Interface
/// This is the main port that driving adapters (Web API) will use
/// </summary>
public interface IAuthService
{
    Task<Result<AuthTokenDto>> RegisterAsync(RegisterUserDto registerDto);
    Task<Result<AuthTokenDto>> LoginAsync(LoginUserDto loginDto);
    Task<Result<AuthTokenDto>> RefreshTokenAsync(string refreshToken);
    Task<Result> LogoutAsync(Guid userId);
    Task<Result> ConfirmEmailAsync(string email, string token);
    Task<Result> ForgotPasswordAsync(string email);
    Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);
}

/// <summary>
/// Primary Port - User Management Service Interface
/// </summary>
public interface IUserService
{
    Task<Result<UserProfileDto>> GetUserProfileAsync(Guid userId);
    Task<Result<UserProfileDto>> UpdateProfileAsync(Guid userId, UpdateUserProfileDto updateDto);
    Task<Result> UpdateProfileImageAsync(Guid userId, string imageUrl);
    Task<Result> DeactivateAccountAsync(Guid userId);
    Task<Result<List<UserSummaryDto>>> SearchUsersAsync(string searchTerm, int page = 1, int pageSize = 10);
}