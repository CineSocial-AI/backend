using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Application.Common.Security;
using CineSocial.Application.Features.Auth.Commands.Login;
using CineSocial.Domain.Entities.User;

namespace CineSocial.Application.UseCases.Auth;

public class LoginUseCase
{
    private readonly IRepository<AppUser> _userRepository;
    private readonly IJwtService _jwtService;

    public LoginUseCase(IRepository<AppUser> userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> ExecuteAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.FindAsync(u => u.Email == email, cancellationToken);
        var user = users.FirstOrDefault();

        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid email or password");

        if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password");

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        var token = _jwtService.GenerateToken(user);

        return new LoginResponse(
            token,
            user.Id,
            user.Username,
            user.Email,
            user.Role.ToString()
        );
    }
}
