using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Security;
using CineSocial.Application.Features.Auth.Commands.Register;
using CineSocial.Domain.Entities.User;
using CineSocial.Domain.Enums;

namespace CineSocial.Application.UseCases.Auth;

public class RegisterUseCase
{
    private readonly IRepository<AppUser> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUseCase(IRepository<AppUser> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterResponse> ExecuteAsync(string username, string email, string password, CancellationToken cancellationToken = default)
    {
        var existingUsers = await _userRepository.FindAsync(u => u.Email == email, cancellationToken);
        if (existingUsers.Any())
            throw new InvalidOperationException("Email already in use");

        var existingUsername = await _userRepository.FindAsync(u => u.Username == username, cancellationToken);
        if (existingUsername.Any())
            throw new InvalidOperationException("Username already in use");

        var user = new AppUser
        {
            Username = username,
            Email = email,
            PasswordHash = PasswordHasher.HashPassword(password),
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterResponse(
            user.Id,
            user.Username,
            user.Email,
            user.Role.ToString()
        );
    }
}
