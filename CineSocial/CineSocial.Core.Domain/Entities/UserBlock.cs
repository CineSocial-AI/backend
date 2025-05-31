using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class UserBlock : BaseEntity
{
    public Guid BlockerId { get; set; }
    public Guid BlockedId { get; set; }

    public virtual User Blocker { get; set; } = null!;
    public virtual User Blocked { get; set; } = null!;
}
