namespace CineSocial.Domain.Entities;

public class MovieListItem : BaseEntity
{
    public Guid MovieListId { get; set; }
    public Guid MovieId { get; set; }
    public int Order { get; set; }
    public string? Notes { get; set; }

    public virtual MovieList MovieList { get; set; } = null!;
    public virtual Movie Movie { get; set; } = null!;
}