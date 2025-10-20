namespace CineSocial.Application.Features.Movies.Queries.GetById;

public record MovieDetailDto
{
    public int Id { get; init; }
    public int TmdbId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? OriginalTitle { get; init; }
    public string? Overview { get; init; }
    public DateTime? ReleaseDate { get; init; }
    public int? Runtime { get; init; }
    public decimal? Budget { get; init; }
    public decimal? Revenue { get; init; }
    public string? PosterPath { get; init; }
    public string? BackdropPath { get; init; }
    public string? ImdbId { get; init; }
    public string? OriginalLanguage { get; init; }
    public double? Popularity { get; init; }
    public double? VoteAverage { get; init; }
    public int? VoteCount { get; init; }
    public string? Status { get; init; }
    public string? Tagline { get; init; }
    public string? Homepage { get; init; }
    public bool Adult { get; init; }

    public List<GenreDto> Genres { get; init; } = new();
    public List<CastDto> Cast { get; init; } = new();
    public List<CrewDto> Crew { get; init; } = new();
    public List<ProductionCompanyDto> ProductionCompanies { get; init; } = new();
    public List<string> Countries { get; init; } = new();
    public List<string> Languages { get; init; } = new();
    public List<string> Keywords { get; init; } = new();
    public List<VideoDto> Videos { get; init; } = new();
    public List<ImageDto> Images { get; init; } = new();
}

public record GenreDto(int Id, string Name);

public record CastDto(
    int Id,
    string Name,
    string? Character,
    int? CastOrder,
    string? ProfilePath
);

public record CrewDto(
    int Id,
    string Name,
    string? Job,
    string? Department,
    string? ProfilePath
);

public record ProductionCompanyDto(
    int Id,
    string Name,
    string? LogoPath,
    string? OriginCountry
);

public record VideoDto(
    string Key,
    string? Name,
    string? Site,
    string? Type,
    bool Official
);

public record ImageDto(
    string FilePath,
    string? ImageType,
    int? Width,
    int? Height
);
