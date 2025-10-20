using CineSocial.Domain.Common;
using CineSocial.Domain.Entities.User;
using CineSocial.Domain.Enums;

namespace CineSocial.Domain.Entities.Social;

public class Comment : BaseEntity
{
    // User who created the comment
    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;

    // Content
    public string Content { get; set; } = string.Empty;

    // What this comment is attached to (Movie or Post)
    public CommentableType CommentableType { get; set; }
    public int CommentableId { get; set; }

    // Nested comments (replies)
    public int? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();

    // Depth level (0 = root comment, 1 = reply, 2 = reply to reply, etc.)
    public int Depth { get; set; }

    // Edit tracking
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }

    // Soft delete (DeletedAt is extra, IsDeleted comes from BaseEntity)
    public DateTime? DeletedAt { get; set; }

    // Navigation
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
}
