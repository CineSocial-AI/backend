using CineSocial.Domain.Common;

namespace CineSocial.Domain.Entities.User;

public class Block : BaseEntity
{
    public int BlockerId { get; set; }
    public AppUser Blocker { get; set; } = null!;

    public int BlockedUserId { get; set; }
    public AppUser BlockedUser { get; set; } = null!;
}
