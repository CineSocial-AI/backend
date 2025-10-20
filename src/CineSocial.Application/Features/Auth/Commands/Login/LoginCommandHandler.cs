using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Application.Common.Security;
using CineSocial.Domain.Entities.User;
using MediatR;

namespace CineSocial.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IRepository<AppUser> _userRepository;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IRepository<AppUser> userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.FindAsync(u => u.Email == request.Email, cancellationToken);
        var user = users.FirstOrDefault();

        if (user == null || !user.IsActive)
            return Result<LoginResponse>.Failure("Invalid email or password");

        if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Result<LoginResponse>.Failure("Invalid email or password");

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        var token = _jwtService.GenerateToken(user);

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
