namespace CineSocial.Domain.Entities.Movie;

public class MovieCast
{
    public int Id { get; set; }

    public int MovieId { get; set; }
    public MovieEntity Movie { get; set; } = null!;

    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;

    public string? Character { get; set; }
    public int? CastOrder { get; set; }
}
