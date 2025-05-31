using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class PostReaction : BaseEntity
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType Type { get; set; }

    public virtual Post Post { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

public enum ReactionType
{
    Upvote = 1,
    Downvote = -1
}
