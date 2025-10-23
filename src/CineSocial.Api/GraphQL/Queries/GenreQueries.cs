using CineSocial.Domain.Entities.Movie;
using CineSocial.Infrastructure.Data;
using HotChocolate;
using HotChocolate.Data;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class GenreQueries
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Genre> GetGenres(
        [Service] ApplicationDbContext context)
    {
        return context.Genres;
    }

    [UseProjection]
    public async Task<Genre?> GetGenreById(
        int id,
        [Service] ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        return await context.Genres.FindAsync(new object[] { id }, cancellationToken);
    }

    [UseProjection]
    public async Task<Genre?> GetGenreByTmdbId(
        int tmdbId,
        [Service] ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        return await Task.FromResult(context.Genres.FirstOrDefault(g => g.TmdbId == tmdbId));
    }
}
