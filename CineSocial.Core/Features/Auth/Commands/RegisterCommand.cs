using CineSocial.Core.Localization;
using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using MediatR;

namespace CineSocial.Core.Features.Auth.Commands;

public record RegisterCommand(
    string Username,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? Bio = null
) : IRequest<Result<LoginResult>>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<LoginResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly ILocalizationService _localizationService;

    public RegisterCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        ILocalizationService localizationService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _localizationService = localizationService;
    }

    public async Task<Result<LoginResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result<LoginResult>.Failure(_localizationService.GetString("User.EmailAlreadyExists"));
        }

        var existingUsername = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
        if (existingUsername != null)
        {
            return Result<LoginResult>.Failure(_localizationService.GetString("User.UsernameAlreadyExists"));
        }

        // Create new user
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Bio = request.Bio,
            IsActive = true,
            IsEmailVerified = false, // In real app, you'd send verification email
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Generate tokens
        var token = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        
        // Update user with refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        user.LastLoginAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtService.GetTokenExpiryMinutes());

        return Result<LoginResult>.Success(new LoginResult(
            token,
            refreshToken,
            expiresAt,
            user
        ));
    }
}