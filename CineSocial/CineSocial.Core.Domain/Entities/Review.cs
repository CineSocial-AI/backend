using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class Review : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public bool IsSpoiler { get; set; }
    public int LikesCount { get; set; }
    public int DislikesCount { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Movie Movie { get; set; } = null!;
    public virtual ICollection<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
