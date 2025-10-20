namespace CineSocial.Domain.Entities.Movie;

public class MovieKeyword
{
    public int MovieId { get; set; }
    public MovieEntity Movie { get; set; } = null!;

    public int KeywordId { get; set; }
    public Keyword Keyword { get; set; } = null!;
}
