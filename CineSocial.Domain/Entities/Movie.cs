namespace CineSocial.Domain.Entities;

public class Movie : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? OriginalTitle { get; set; }
    public string? Overview { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public int? Runtime { get; set; }
    public decimal? VoteAverage { get; set; }
    public int? VoteCount { get; set; }
    public string? PosterPath { get; set; }
    public string? BackdropPath { get; set; }
    public string? Language { get; set; }
    public decimal? Popularity { get; set; }
    public bool IsAdult { get; set; }
    public string? Homepage { get; set; }
    public string? Status { get; set; }
    public long? Budget { get; set; }
    public long? Revenue { get; set; }
    public string? Tagline { get; set; }
    public string? ImdbId { get; set; }
    public int? TmdbId { get; set; }

    public virtual ICollection<MovieCast> MovieCasts { get; set; } = new List<MovieCast>();
    public virtual ICollection<MovieCrew> MovieCrews { get; set; } = new List<MovieCrew>();
    public virtual ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public virtual ICollection<MovieListItem> MovieListItems { get; set; } = new List<MovieListItem>();
}