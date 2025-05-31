using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class GroupBan : BaseEntity
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public Guid BannedById { get; set; }
    public string? Reason { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }

    public virtual Group Group { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual User BannedBy { get; set; } = null!;
}
