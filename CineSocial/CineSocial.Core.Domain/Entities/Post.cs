using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class Post : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public PostType Type { get; set; }
    public string? Url { get; set; }
    public bool IsNsfw { get; set; }
    public bool IsSpoiler { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsLocked { get; set; }
    public int UpvoteCount { get; set; }
    public int DownvoteCount { get; set; }
    public int CommentCount { get; set; }
    public int ViewCount { get; set; }
    public Guid AuthorId { get; set; }
    public Guid GroupId { get; set; }

    public virtual User Author { get; set; } = null!;
    public virtual Group Group { get; set; } = null!;
    public virtual ICollection<PostMedia> Media { get; set; } = new List<PostMedia>();
    public virtual ICollection<PostReaction> Reactions { get; set; } = new List<PostReaction>();
    public virtual ICollection<PostComment> Comments { get; set; } = new List<PostComment>();
    public virtual ICollection<PostTag> Tags { get; set; } = new List<PostTag>();
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public int GetScore() => UpvoteCount - DownvoteCount;
}

public enum PostType
{
    Text = 1,
    Image = 2,
    Video = 3,
    Link = 4,
    Poll = 5
}
