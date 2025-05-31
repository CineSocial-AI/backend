using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class ReviewLike : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ReviewId { get; set; }
    public bool IsLike { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Review Review { get; set; } = null!;
}
