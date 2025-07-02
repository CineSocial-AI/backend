namespace CineSocial.Domain.Entities;

public class Favorite : BaseEntity
{
    public Guid MovieId { get; set; }
    public Guid UserId { get; set; }

    public virtual Movie Movie { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}