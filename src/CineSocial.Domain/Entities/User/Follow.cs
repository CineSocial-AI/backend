using CineSocial.Domain.Common;

namespace CineSocial.Domain.Entities.User;

public class Follow : BaseEntity
{
    public int FollowerId { get; set; }
    public AppUser Follower { get; set; } = null!;

    public int FollowingId { get; set; }
    public AppUser Following { get; set; } = null!;
}
