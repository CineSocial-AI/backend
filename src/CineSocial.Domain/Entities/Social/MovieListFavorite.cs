namespace CineSocial.Domain.Entities.Social;

public class MovieListFavorite
{
    public int UserId { get; set; }
    public int MovieListId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User.AppUser User { get; set; } = null!;
    public virtual MovieList MovieList { get; set; } = null!;
}
