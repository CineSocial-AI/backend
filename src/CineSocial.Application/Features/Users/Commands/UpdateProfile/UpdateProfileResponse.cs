namespace CineSocial.Application.Features.Users.Commands.UpdateProfile;

public record UpdateProfileResponse(
    int UserId,
    string Username,
    string Email,
    string? Bio,
    int? ProfileImageId,
    int? BackgroundImageId
);
