namespace CineSocial.Core.Features.Movies.DTOs;

public record MovieDto(
    Guid Id,
    string Title,
    string? OriginalTitle,
    string? Overview,
    DateTime? ReleaseDate,
    int? Runtime,
    decimal? VoteAverage,
    int? VoteCount,
    string? PosterPath,
    string? BackdropPath,
    string? Language,
    decimal? Popularity,
    bool IsAdult,
    string? Homepage,
    string? Status,
    long? Budget,
    long? Revenue,
    string? Tagline,
    string? ImdbId,
    int? TmdbId,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateMovieDto(
    string Title,
    string? OriginalTitle,
    string? Overview,
    DateTime? ReleaseDate,
    int? Runtime,
    decimal? VoteAverage,
    int? VoteCount,
    string? PosterPath,
    string? BackdropPath,
    string? Language,
    decimal? Popularity,
    bool IsAdult,
    string? Homepage,
    string? Status,
    long? Budget,
    long? Revenue,
    string? Tagline,
    string? ImdbId,
    int? TmdbId
);

public record UpdateMovieDto(
    Guid Id,
    string Title,
    string? OriginalTitle,
    string? Overview,
    DateTime? ReleaseDate,
    int? Runtime,
    decimal? VoteAverage,
    int? VoteCount,
    string? PosterPath,
    string? BackdropPath,
    string? Language,
    decimal? Popularity,
    bool IsAdult,
    string? Homepage,
    string? Status,
    long? Budget,
    long? Revenue,
    string? Tagline,
    string? ImdbId,
    int? TmdbId
);

public record MovieListDto(
    Guid Id,
    string Title,
    string? OriginalTitle,
    DateTime? ReleaseDate,
    decimal? VoteAverage,
    string? PosterPath,
    decimal? Popularity
);