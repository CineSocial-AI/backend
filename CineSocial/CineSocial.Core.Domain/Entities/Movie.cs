using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

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

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    public virtual ICollection<MovieCast> MovieCasts { get; set; } = new List<MovieCast>();
    public virtual ICollection<MovieCrew> MovieCrews { get; set; } = new List<MovieCrew>();
    public virtual ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public decimal GetAverageRating()
    {
        return Reviews.Any() ? Reviews.Average(r => r.Rating) : 0;
    }

    public int GetReviewCount()
    {
        return Reviews.Count;
    }
}
