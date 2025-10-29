namespace CineSocial.Application.Features.Users.Queries.GetAll;

public record UserSummaryDto(
    int Id,
    string Username,
    string Email,
    string? Bio
);
