using System.ComponentModel.DataAnnotations;

namespace CineSocial.Core.Application.DTOs.Watchlists;

public class WatchlistDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    public bool IsWatched { get; set; }
    public DateTime? WatchedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string? MoviePoster { get; set; }
    public DateTime? MovieReleaseDate { get; set; }
    public decimal? MovieRating { get; set; }
    public List<string> MovieGenres { get; set; } = new();
}

public class AddToWatchlistDto
{
    [Required]
    public Guid MovieId { get; set; }
}

public class UpdateWatchlistDto
{
    public bool IsWatched { get; set; }
    public DateTime? WatchedDate { get; set; }
}

public class WatchlistSummaryDto
{
    public int TotalMovies { get; set; }
    public int WatchedMovies { get; set; }
    public int UnwatchedMovies { get; set; }
    public List<WatchlistDto> RecentlyAdded { get; set; } = new();
    public List<WatchlistDto> RecentlyWatched { get; set; } = new();
}
