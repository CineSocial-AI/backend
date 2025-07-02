namespace CineSocial.Domain.Entities;

public class Comment : BaseEntity
{
    public Guid ReviewId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
    public int UpvotesCount { get; set; }
    public int DownvotesCount { get; set; }

    public virtual Review Review { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual Comment? ParentComment { get; set; }
    public virtual ICollection<Comment> ChildComments { get; set; } = new List<Comment>();
    public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
}