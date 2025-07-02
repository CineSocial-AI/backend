namespace CineSocial.Domain.Entities;

public class MovieList : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsWatchlist { get; set; }
    public bool IsPublic { get; set; } = true;

    public virtual User User { get; set; } = null!;
    public virtual ICollection<MovieListItem> MovieListItems { get; set; } = new List<MovieListItem>();
    public virtual ICollection<ListFavorite> ListFavorites { get; set; } = new List<ListFavorite>();
}