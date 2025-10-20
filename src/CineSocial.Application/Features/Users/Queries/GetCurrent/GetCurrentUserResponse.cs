namespace CineSocial.Application.Features.Users.Queries.GetCurrent;

public record GetCurrentUserResponse(
    int Id,
    string Username,
    string Email,
    string Role,
    string? Bio,
    int? ProfileImageId,
    int? BackgroundImageId,
    DateTime? LastLoginAt,
    DateTime CreatedAt
);
