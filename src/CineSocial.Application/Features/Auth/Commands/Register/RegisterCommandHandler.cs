using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Application.Common.Security;
using CineSocial.Domain.Entities.Social;
using CineSocial.Domain.Entities.User;
using CineSocial.Domain.Enums;
using MediatR;

namespace CineSocial.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IRepository<AppUser> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _dbContext;

    public RegisterCommandHandler(
        IRepository<AppUser> userRepository,
        IUnitOfWork unitOfWork,
        IApplicationDbContext dbContext)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
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

        // Create default Watchlist for new user
        var watchlist = new MovieList
        {
            UserId = user.Id,
            Name = "Watchlist",
            Description = "My movies to watch",
            IsPublic = true,
            IsWatchlist = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        _dbContext.Add(watchlist);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new RegisterResponse(
            user.Id,
            user.Username,
            user.Email,
            user.Role.ToString()
        );

        return Result<RegisterResponse>.Success(response, "Registration successful");
    }
}
