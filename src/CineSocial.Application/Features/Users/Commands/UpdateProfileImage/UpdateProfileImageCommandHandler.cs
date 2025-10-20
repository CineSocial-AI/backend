using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.User;
using MediatR;

namespace CineSocial.Application.Features.Users.Commands.UpdateProfileImage;

public class UpdateProfileImageCommandHandler : IRequestHandler<UpdateProfileImageCommand, Result<UpdateProfileImageResponse>>
{
    private readonly IRepository<AppUser> _userRepository;
    private readonly IRepository<Image> _imageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileImageCommandHandler(
        IRepository<AppUser> userRepository,
        IRepository<Image> imageRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _imageRepository = imageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UpdateProfileImageResponse>> Handle(UpdateProfileImageCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return Result<UpdateProfileImageResponse>.Failure("User not found");

        if (request.Data == null || request.Data.Length == 0)
            return Result<UpdateProfileImageResponse>.Failure("No image data provided");

        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(request.ContentType.ToLower()))
            return Result<UpdateProfileImageResponse>.Failure("Invalid image type");

        if (request.Data.Length > 5 * 1024 * 1024)
            return Result<UpdateProfileImageResponse>.Failure("Image size must be less than 5MB");

        var image = new Image
        {
            FileName = request.FileName,
            ContentType = request.ContentType,
            Data = request.Data,
            Size = request.Data.Length,
            CreatedAt = DateTime.UtcNow
        };

        await _imageRepository.AddAsync(image, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        user.ProfileImageId = image.Id;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new UpdateProfileImageResponse(user.Id, image.Id);

        return Result<UpdateProfileImageResponse>.Success(response, "Profile image updated successfully");
    }
}
