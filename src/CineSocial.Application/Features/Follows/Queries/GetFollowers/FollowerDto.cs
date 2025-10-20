namespace CineSocial.Application.Features.Follows.Queries.GetFollowers;

public class FollowerDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public int? ProfileImageId { get; set; }
    public DateTime FollowedAt { get; set; }
}
