using CineSocial.Domain.Entities.Social;

namespace CineSocial.Api.GraphQL.Types;

public class MovieListType
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CoverImageId { get; set; }
    public bool IsPublic { get; set; }
    public bool IsWatchlist { get; set; }
    public int FavoriteCount { get; set; }
    public int MovieCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static MovieListType FromEntity(MovieList entity)
    {
        return new MovieListType
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Name = entity.Name,
            Description = entity.Description,
            CoverImageId = entity.CoverImageId,
            IsPublic = entity.IsPublic,
            IsWatchlist = entity.IsWatchlist,
            FavoriteCount = entity.FavoriteCount,
            MovieCount = entity.Items?.Count ?? 0,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
