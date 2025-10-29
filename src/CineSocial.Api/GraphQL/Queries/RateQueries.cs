using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using HotChocolate;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class RateQueries
{
    /// <summary>
    /// Get all rates for a specific movie
    /// </summary>
    [UseProjection]
    public IQueryable<Rate> GetMovieRates(
        int movieId,
        [Service] IRepository<Rate> repository)
    {
        return repository.GetQueryable()
            .Where(r => r.MovieId == movieId)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt);
    }

    /// <summary>
    /// Get user's rate for a specific movie
    /// </summary>
    public async Task<Rate?> GetUserRateForMovie(
        int movieId,
        int userId,
        [Service] IRepository<Rate> repository,
        CancellationToken cancellationToken)
    {
        return await repository.GetQueryable()
            .FirstOrDefaultAsync(r => r.MovieId == movieId && r.UserId == userId, cancellationToken);
    }

    /// <summary>
    /// Get average rating for a movie
    /// </summary>
    public async Task<decimal?> GetMovieAverageRating(
        int movieId,
        [Service] IRepository<Rate> repository,
        CancellationToken cancellationToken)
    {
        var rates = await repository.GetQueryable()
            .Where(r => r.MovieId == movieId)
            .ToListAsync(cancellationToken);

        return rates.Any() ? rates.Average(r => r.Rating) : null;
    }

    /// <summary>
    /// Get all rates by a user
    /// </summary>
    [UseProjection]
    public IQueryable<Rate> GetUserRates(
        int userId,
        [Service] IRepository<Rate> repository)
    {
        return repository.GetQueryable()
            .Where(r => r.UserId == userId)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt);
    }
}
