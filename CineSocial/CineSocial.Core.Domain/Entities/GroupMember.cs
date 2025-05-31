using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class GroupMember : BaseEntity
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public GroupRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }

    public virtual Group Group { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

public enum GroupRole
{
    Member = 1,
    Moderator = 2,
    Admin = 3,
    Owner = 4
}
