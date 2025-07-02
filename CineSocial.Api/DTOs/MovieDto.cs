using System.ComponentModel.DataAnnotations;

namespace CineSocial.Api.DTOs;

public class MovieDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string OriginalTitle { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public int Runtime { get; set; }
    public decimal VoteAverage { get; set; }
    public int VoteCount { get; set; }
    public string Language { get; set; } = string.Empty;
    public decimal Popularity { get; set; }
    public string Status { get; set; } = string.Empty;
    public long Budget { get; set; }
    public long Revenue { get; set; }
    public string? Tagline { get; set; }
    public List<GenreDto> Genres { get; set; } = new();
    public List<MovieCastDto> Cast { get; set; } = new();
    public List<MovieCrewDto> Crew { get; set; } = new();
}

public class MovieListDto
{
    public List<MovieDto> Movies { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class MovieSearchRequest
{
    [StringLength(100)]
    public string? Query { get; set; }

    public List<Guid>? GenreIds { get; set; }

    [Range(1900, 2030)]
    public int? Year { get; set; }

    [Range(1, 100)]
    public int Page { get; set; } = 1;

    [Range(1, 50)]
    public int PageSize { get; set; } = 10;

    public string? SortBy { get; set; } = "popularity";

    public string? SortOrder { get; set; } = "desc";
}

public class GenreDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class MovieCastDto
{
    public Guid PersonId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Character { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class MovieCrewDto
{
    public Guid PersonId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}