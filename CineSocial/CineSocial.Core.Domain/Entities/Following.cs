using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class Following : BaseEntity
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }

    public virtual User Follower { get; set; } = null!;
    public virtual User FollowedUser { get; set; } = null!;
}