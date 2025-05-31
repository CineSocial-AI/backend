using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class PostComment : BaseEntity
{
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public bool IsEdited { get; set; }
    public int UpvoteCount { get; set; }
    public int DownvoteCount { get; set; }
    public int ReplyCount { get; set; }

    public virtual Post Post { get; set; } = null!;
    public virtual User Author { get; set; } = null!;
    public virtual PostComment? ParentComment { get; set; }
    public virtual ICollection<PostComment> Replies { get; set; } = new List<PostComment>();
    public virtual ICollection<CommentReaction> Reactions { get; set; } = new List<CommentReaction>();
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public int GetScore() => UpvoteCount - DownvoteCount;
}
