using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<LoginResult>>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<Result<LoginResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByRefreshTokenAsync(request.RefreshToken);
        if (user == null)
        {
            return Result<LoginResult>.Failure("Geçersiz refresh token.");
        }

        if (user.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            return Result<LoginResult>.Failure("Refresh token süresi dolmuş.");
        }

        if (!user.IsActive)
        {
            return Result<LoginResult>.Failure("Hesap aktif değil.");
        }

        // Generate new tokens
        var token = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        
        // Update user
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