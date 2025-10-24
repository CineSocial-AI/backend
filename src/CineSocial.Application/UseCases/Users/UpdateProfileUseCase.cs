using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Features.Users.Commands.UpdateProfile;
using CineSocial.Domain.Entities.User;

namespace CineSocial.Application.UseCases.Users;

public class UpdateProfileUseCase
{
    private readonly IRepository<AppUser> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProfileUseCase(IRepository<AppUser> userRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<UpdateProfileResponse> ExecuteAsync(
        string? username,
        string? bio,
        int? profileImageId,
        int? backgroundImageId,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user == null)
            throw new NotFoundException("User", userId);

        if (!string.IsNullOrWhiteSpace(username) && username != user.Username)
        {
            var existingUser = await _userRepository.FindAsync(u => u.Username == username, cancellationToken);
            if (existingUser.Any())
                throw new ConflictException("Username already taken");

            user.Username = username;
        }

        if (bio != null)
            user.Bio = bio;

        if (profileImageId.HasValue)
            user.ProfileImageId = profileImageId.Value;

        if (backgroundImageId.HasValue)
            user.BackgroundImageId = backgroundImageId.Value;

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateProfileResponse(
            user.Id,
            user.Username,
            user.Email,
            user.Bio,
            user.ProfileImageId,
            user.BackgroundImageId
        );
    }
}
