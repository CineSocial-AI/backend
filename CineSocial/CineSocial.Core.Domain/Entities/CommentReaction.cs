using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class CommentReaction : BaseEntity
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType Type { get; set; }

    public virtual PostComment Comment { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
