namespace CineSocial.Api.DTOs;

public record CreateMovieListRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsPublic { get; init; } = true;
}

public record UpdateMovieListRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsPublic { get; init; } = true;
}

public record AddMovieToListRequest
{
    public Guid MovieId { get; init; }
    public string? Notes { get; init; }
}

public record UserMovieListDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsPublic { get; init; }
    public bool IsWatchlist { get; init; }
    public int MovieCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string UserFullName { get; init; } = string.Empty;
    public string UserUsername { get; init; } = string.Empty;
    public bool IsFavorited { get; init; }
}

public record MovieListDetailDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsPublic { get; init; }
    public bool IsWatchlist { get; init; }
    public List<MovieListItemDto> Movies { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string UserFullName { get; init; } = string.Empty;
    public string UserUsername { get; init; } = string.Empty;
    public bool IsFavorited { get; init; }
}

public record MovieListItemDto
{
    public Guid Id { get; init; }
    public Guid MovieId { get; init; }
    public string MovieTitle { get; init; } = string.Empty;
    public string MoviePosterPath { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    public int Order { get; init; }
    public DateTime AddedAt { get; init; }
}

public record PagedUserMovieListDto
{
    public List<UserMovieListDto> MovieLists { get; init; } = new();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}