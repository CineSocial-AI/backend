using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Logging;
using CineSocial.Application.Common.Models;
using CineSocial.Application.Common.Security;
using CineSocial.Domain.Entities.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CineSocial.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IRepository<AppUser> _userRepository;
    private readonly IJwtService _jwtService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IRepository<AppUser> userRepository,
        IJwtService jwtService,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var maskedEmail = SensitiveDataMasker.MaskEmail(request.Email);
        _logger.LogInformation("Login attempt for email: {MaskedEmail}", maskedEmail);

        var users = await _userRepository.FindAsync(u => u.Email == request.Email, cancellationToken);
        var user = users.FirstOrDefault();

        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("Login failed for email: {MaskedEmail} - User not found or inactive", maskedEmail);
            return Result<LoginResponse>.Failure("Invalid email or password");
        }

        if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed for email: {MaskedEmail} - Invalid password", maskedEmail);
            return Result<LoginResponse>.Failure("Invalid email or password");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        var token = _jwtService.GenerateToken(user);

        _logger.LogInformation("Login successful for user: {UserId} ({Username})", user.Id, user.Username);

        var response = new LoginResponse(
            token,
            user.Id,
            user.Username,
            user.Email,
            user.Role.ToString()
        );

        return Result<LoginResponse>.Success(response, "Login successful");
    }
}
