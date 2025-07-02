namespace CineSocial.Domain.Entities;

public class Review : BaseEntity
{
    public Guid MovieId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool ContainsSpoilers { get; set; }
    public int LikesCount { get; set; }

    public virtual Movie Movie { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}