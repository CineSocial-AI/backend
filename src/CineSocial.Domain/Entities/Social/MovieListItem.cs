using CineSocial.Domain.Common;
using CineSocial.Domain.Entities.Movie;

namespace CineSocial.Domain.Entities.Social;

public class MovieListItem : BaseEntity
{
    public int MovieListId { get; set; }
    public int MovieId { get; set; }
    public int Order { get; set; } = 0;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual MovieList MovieList { get; set; } = null!;
    public virtual MovieEntity Movie { get; set; } = null!;
}
