using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.User;
using MediatR;

namespace CineSocial.Application.Features.Users.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<UpdateProfileResponse>>
{
    private readonly IRepository<AppUser> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(IRepository<AppUser> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UpdateProfileResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
            return Result<UpdateProfileResponse>.Failure("User not found");

        if (!string.IsNullOrWhiteSpace(request.Username) && request.Username != user.Username)
        {
            var existingUser = await _userRepository.FindAsync(u => u.Username == request.Username, cancellationToken);
            if (existingUser.Any())
                return Result<UpdateProfileResponse>.Failure("Username already taken");

            user.Username = request.Username;
        }

        if (request.Bio != null)
            user.Bio = request.Bio;

        if (request.ProfileImageId.HasValue)
            user.ProfileImageId = request.ProfileImageId.Value;

        if (request.BackgroundImageId.HasValue)
            user.BackgroundImageId = request.BackgroundImageId.Value;

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new UpdateProfileResponse(
            user.Id,
            user.Username,
            user.Email,
            user.Bio,
            user.ProfileImageId,
            user.BackgroundImageId
        );

        return Result<UpdateProfileResponse>.Success(response, "Profile updated successfully");
    }
}
