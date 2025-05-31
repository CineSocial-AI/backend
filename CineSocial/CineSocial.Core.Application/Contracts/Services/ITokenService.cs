using CineSocial.Core.Domain.Entities;

namespace CineSocial.Core.Application.Contracts.Services;

/// <summary>
/// Secondary Port - Token Service Interface
/// This will be implemented by infrastructure layer
/// </summary>
public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    Guid? GetUserIdFromToken(string token);
    DateTime GetTokenExpiration(string token);
}

/// <summary>
/// Secondary Port - Email Service Interface
/// </summary>
public interface IEmailService
{
    Task SendEmailConfirmationAsync(string email, string confirmationLink);
    Task SendPasswordResetAsync(string email, string resetLink);
    Task SendWelcomeEmailAsync(string email, string firstName);
}

/// <summary>
/// Secondary Port - File Storage Service Interface
/// </summary>
public interface IFileStorageService
{
    Task<string> UploadProfileImageAsync(Stream imageStream, string fileName, string userId);
    Task DeleteFileAsync(string fileUrl);
    Task<bool> IsValidImageAsync(Stream imageStream);
}

/// <summary>
/// Secondary Port - Current User Service Interface
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}