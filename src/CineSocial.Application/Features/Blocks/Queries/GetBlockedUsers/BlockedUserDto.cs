namespace CineSocial.Application.Features.Blocks.Queries.GetBlockedUsers;

public class BlockedUserDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public int? ProfileImageId { get; set; }
    public DateTime BlockedAt { get; set; }
}
