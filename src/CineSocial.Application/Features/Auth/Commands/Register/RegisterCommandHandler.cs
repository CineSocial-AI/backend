using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Application.Common.Security;
using CineSocial.Domain.Entities.User;
using CineSocial.Domain.Enums;
using MediatR;

namespace CineSocial.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IRepository<AppUser> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(IRepository<AppUser> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUsers = await _userRepository.FindAsync(u => u.Email == request.Email, cancellationToken);
        if (existingUsers.Any())
            return Result<RegisterResponse>.Failure("Email already in use");

        var existingUsername = await _userRepository.FindAsync(u => u.Username == request.Username, cancellationToken);
        if (existingUsername.Any())
            return Result<RegisterResponse>.Failure("Username already in use");

        var user = new AppUser
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new RegisterResponse(
            user.Id,
            user.Username,
            user.Email,
            user.Role.ToString()
        );

        return Result<RegisterResponse>.Success(response, "Registration successful");
    }
}
