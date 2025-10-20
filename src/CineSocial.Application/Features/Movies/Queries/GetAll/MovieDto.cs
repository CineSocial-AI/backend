namespace CineSocial.Application.Features.Movies.Queries.GetAll;

public record MovieDto
{
    public int Id { get; init; }
    public int TmdbId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? OriginalTitle { get; init; }
    public string? Overview { get; init; }
    public DateTime? ReleaseDate { get; init; }
    public int? Runtime { get; init; }
    public string? PosterPath { get; init; }
    public string? BackdropPath { get; init; }
    public double? Popularity { get; init; }
    public double? VoteAverage { get; init; }
    public int? VoteCount { get; init; }
    public List<string> Genres { get; init; } = new();
}
