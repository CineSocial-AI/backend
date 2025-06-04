using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class Comment : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ReviewId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int LikeCount { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Review Review { get; set; } = null!;
    public virtual Comment? ParentComment { get; set; }
    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
    public virtual ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();
}
