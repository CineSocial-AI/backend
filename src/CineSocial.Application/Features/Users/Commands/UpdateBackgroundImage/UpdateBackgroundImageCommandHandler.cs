using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.User;
using MediatR;

namespace CineSocial.Application.Features.Users.Commands.UpdateBackgroundImage;

public class UpdateBackgroundImageCommandHandler : IRequestHandler<UpdateBackgroundImageCommand, Result<UpdateBackgroundImageResponse>>
{
    private readonly IRepository<AppUser> _userRepository;
    private readonly IRepository<Image> _imageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBackgroundImageCommandHandler(
        IRepository<AppUser> userRepository,
        IRepository<Image> imageRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _imageRepository = imageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UpdateBackgroundImageResponse>> Handle(UpdateBackgroundImageCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return Result<UpdateBackgroundImageResponse>.Failure("User not found");

        if (request.Data == null || request.Data.Length == 0)
            return Result<UpdateBackgroundImageResponse>.Failure("No image data provided");

        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(request.ContentType.ToLower()))
            return Result<UpdateBackgroundImageResponse>.Failure("Invalid image type");

        if (request.Data.Length > 5 * 1024 * 1024)
            return Result<UpdateBackgroundImageResponse>.Failure("Image size must be less than 5MB");

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

        user.BackgroundImageId = image.Id;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new UpdateBackgroundImageResponse(user.Id, image.Id);

        return Result<UpdateBackgroundImageResponse>.Success(response, "Background image updated successfully");
    }
}
