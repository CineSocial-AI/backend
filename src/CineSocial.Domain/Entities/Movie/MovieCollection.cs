namespace CineSocial.Domain.Entities.Movie;

public class MovieCollection
{
    public int MovieId { get; set; }
    public MovieEntity Movie { get; set; } = null!;

    public int CollectionId { get; set; }
    public Collection Collection { get; set; } = null!;
}
