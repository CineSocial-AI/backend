using System.Security.Claims;
using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using HotChocolate;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class MovieListQueries
{
    /// <summary>
    /// Get a movie list by ID with all its items
    /// </summary>
    public async Task<MovieList?> GetMovieList(
        int id,
        [Service] IRepository<MovieList> repository,
        CancellationToken cancellationToken)
    {
        return await repository.GetQueryable()
            .Include(ml => ml.User)
            .Include(ml => ml.Items)
                .ThenInclude(i => i.Movie)
            .Include(ml => ml.Favorites)
            .FirstOrDefaultAsync(ml => ml.Id == id, cancellationToken);
    }

    /// <summary>
    /// Get current user's movie lists
    /// </summary>
    [UseProjection]
    public IQueryable<MovieList> GetMyMovieLists(
        [Service] IRepository<MovieList> repository,
        [Service] IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? user?.FindFirst("sub")?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            throw new Exception("User not authenticated");
        }

        return repository.GetQueryable()
            .Where(ml => ml.UserId == userId)
            .Include(ml => ml.Items)
                .ThenInclude(i => i.Movie)
            .OrderByDescending(ml => ml.UpdatedAt ?? ml.CreatedAt);
    }

    /// <summary>
    /// Get movie lists of a specific user (only public ones if not the owner)
    /// </summary>
    [UseProjection]
    public IQueryable<MovieList> GetUserMovieLists(
        int userId,
        [Service] IRepository<MovieList> repository)
    {
        return repository.GetQueryable()
            .Where(ml => ml.UserId == userId && ml.IsPublic)
            .Include(ml => ml.Items)
                .ThenInclude(i => i.Movie)
            .OrderByDescending(ml => ml.UpdatedAt ?? ml.CreatedAt);
    }

    /// <summary>
    /// Get all public movie lists
    /// </summary>
    [UseProjection]
    public IQueryable<MovieList> GetPublicMovieLists(
        [Service] IRepository<MovieList> repository)
    {
        return repository.GetQueryable()
            .Where(ml => ml.IsPublic)
            .Include(ml => ml.User)
            .Include(ml => ml.Items)
                .ThenInclude(i => i.Movie)
            .OrderByDescending(ml => ml.FavoriteCount)
            .ThenByDescending(ml => ml.UpdatedAt ?? ml.CreatedAt);
    }

    /// <summary>
    /// Get current user's watchlist
    /// </summary>
    public async Task<MovieList?> GetMyWatchlist(
        [Service] IRepository<MovieList> repository,
        [Service] IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? user?.FindFirst("sub")?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            throw new Exception("User not authenticated");
        }

        return await repository.GetQueryable()
            .Where(ml => ml.UserId == userId && ml.IsWatchlist)
            .Include(ml => ml.Items)
                .ThenInclude(i => i.Movie)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Get lists favorited by current user
    /// </summary>
    [UseProjection]
    public IQueryable<MovieList> GetMyFavoriteLists(
        [Service] IRepository<MovieListFavorite> favRepository,
        [Service] IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? user?.FindFirst("sub")?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            throw new Exception("User not authenticated");
        }

        return favRepository.GetQueryable()
            .Where(f => f.UserId == userId)
            .Select(f => f.MovieList)
            .Include(ml => ml.User)
            .Include(ml => ml.Items)
                .ThenInclude(i => i.Movie);
    }
}
