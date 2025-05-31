using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class CommentLike : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid CommentId { get; set; }
    public bool IsLike { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Comment Comment { get; set; } = null!;
}
