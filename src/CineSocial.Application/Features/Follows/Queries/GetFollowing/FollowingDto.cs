namespace CineSocial.Application.Features.Follows.Queries.GetFollowing;

public class FollowingDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public int? ProfileImageId { get; set; }
    public DateTime FollowedAt { get; set; }
}
