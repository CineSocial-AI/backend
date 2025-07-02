using CineSocial.Domain.Enums;

namespace CineSocial.Domain.Entities;

public class Reaction : BaseEntity
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType Type { get; set; }

    public virtual Comment Comment { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}