using System.ComponentModel.DataAnnotations;

namespace CineSocial.Core.Application.DTOs.Movies;

public class MovieDto
{
    public Guid Id { get; set; }
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
    public string? Status { get; set; }
    public string? Tagline { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public List<GenreDto> Genres { get; set; } = new();
    public List<CastMemberDto> Cast { get; set; } = new();
    public List<CrewMemberDto> Crew { get; set; } = new();
}

public class MovieSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PosterPath { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public decimal? VoteAverage { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public List<string> Genres { get; set; } = new();
}

public class CreateMovieDto
{
    [Required]
    [MinLength(1)]
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
    public List<Guid> GenreIds { get; set; } = new();
}

public class UpdateMovieDto
{
    [Required]
    [MinLength(1)]
    public string Title { get; set; } = string.Empty;
    
    public string? OriginalTitle { get; set; }
    public string? Overview { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public int? Runtime { get; set; }
    public string? PosterPath { get; set; }
    public string? BackdropPath { get; set; }
    public string? Status { get; set; }
    public string? Tagline { get; set; }
    public List<Guid> GenreIds { get; set; } = new();
}

public class GenreDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateGenreDto
{
    [Required]
    [MinLength(1)]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    public int? TmdbId { get; set; }
}

public class CastMemberDto
{
    public Guid PersonId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Character { get; set; } = string.Empty;
    public string? ProfilePath { get; set; }
    public int Order { get; set; }
}

public class CrewMemberDto
{
    public Guid PersonId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string? ProfilePath { get; set; }
}

public class PersonDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public DateTime? Birthday { get; set; }
    public DateTime? Deathday { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? ProfilePath { get; set; }
    public string? Gender { get; set; }
    public string? KnownForDepartment { get; set; }
}

public class CreatePersonDto
{
    [Required]
    [MinLength(1)]
    public string Name { get; set; } = string.Empty;
    
    public string? Biography { get; set; }
    public DateTime? Birthday { get; set; }
    public DateTime? Deathday { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? ProfilePath { get; set; }
    public int? TmdbId { get; set; }
    public string? ImdbId { get; set; }
    public decimal? Popularity { get; set; }
    public string? Gender { get; set; }
    public string? KnownForDepartment { get; set; }
}
