using CineSocial.Core.Application.DTOs.Auth;
using CineSocial.Core.Application.DTOs.Common;

namespace CineSocial.Core.Application.Services;

/// <summary>
/// Application Service - Authentication Service Interface
/// This service encapsulates authentication business logic
/// </summary>
public interface IAuthService
{
    Task<Result<AuthTokenDto>> RegisterAsync(RegisterUserDto registerDto, CancellationToken cancellationToken = default);
    Task<Result<AuthTokenDto>> LoginAsync(LoginUserDto loginDto, CancellationToken cancellationToken = default);
    Task<Result<AuthTokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken = default);
    Task<Result> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto, CancellationToken cancellationToken = default);
}