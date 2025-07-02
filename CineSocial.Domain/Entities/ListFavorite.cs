namespace CineSocial.Domain.Entities;

public class ListFavorite : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid MovieListId { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual MovieList MovieList { get; set; } = null!;
}