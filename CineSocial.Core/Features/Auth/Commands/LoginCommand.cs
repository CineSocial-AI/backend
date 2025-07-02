using CineSocial.Core.Localization;
using CineSocial.Core.Logging;
using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CineSocial.Core.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResult>>;

public record LoginResult(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt,
    User User
);

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly ILocalizationService _localizationService;
    private readonly IAuthenticationLogger _authLogger;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork, 
        IPasswordHasher passwordHasher, 
        IJwtService jwtService,
        ILocalizationService localizationService,
        IAuthenticationLogger authLogger,
        ILogger<LoginCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _localizationService = localizationService;
        _authLogger = authLogger;
        _logger = logger;
    }

    public async Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);
        
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (user == null)
        {
            _authLogger.LogFailedLogin(request.Email, "Unknown", "User not found");
            _logger.LogWarning("Login failed - user not found: {Email}", request.Email);
            return Result<LoginResult>.Failure(_localizationService.GetString("User.NotFound"));
        }

        if (!user.IsActive)
        {
            _authLogger.LogFailedLogin(request.Email, "Unknown", "Account inactive");
            _logger.LogWarning("Login failed - account inactive: {Email}, UserId: {UserId}", request.Email, user.Id);
            return Result<LoginResult>.Failure(_localizationService.GetString("User.InactiveAccount"));
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            _authLogger.LogFailedLogin(request.Email, "Unknown", "Invalid password");
            _logger.LogWarning("Login failed - invalid password: {Email}, UserId: {UserId}", request.Email, user.Id);
            return Result<LoginResult>.Failure(_localizationService.GetString("User.InvalidCredentials"));
        }

        var token = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        
        // Update user's last login and refresh token
        user.LastLoginAt = DateTime.UtcNow;
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7); // 7 days

        await _unitOfWork.SaveChangesAsync();

        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtService.GetTokenExpiryMinutes());
        
        // Log successful login
        _authLogger.LogSuccessfulLogin(user.Id.ToString(), user.Username, "Unknown");
        _logger.LogInformation("Login successful: {Email}, UserId: {UserId}, Username: {Username}", 
            request.Email, user.Id, user.Username);

        return Result<LoginResult>.Success(new LoginResult(
            token,
            refreshToken,
            expiresAt,
            user
        ));
    }
}