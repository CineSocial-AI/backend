using CineSocial.Domain.Entities.Movie;
using CineSocial.Infrastructure.Data;
using HotChocolate;
using HotChocolate.Data;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class CollectionQueries
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Collection> GetCollections(
        [Service] ApplicationDbContext context,
        string? searchTerm = null)
    {
        var query = context.Collections.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c => c.Name.Contains(searchTerm));
        }

        return query;
    }

    [UseProjection]
    public async Task<Collection?> GetCollectionById(
        int id,
        [Service] ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        return await context.Collections.FindAsync(new object[] { id }, cancellationToken);
    }

    [UseProjection]
    public async Task<Collection?> GetCollectionByTmdbId(
        int tmdbId,
        [Service] ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        return await Task.FromResult(context.Collections.FirstOrDefault(c => c.TmdbId == tmdbId));
    }
}
