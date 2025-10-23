using CineSocial.Domain.Entities.Movie;
using CineSocial.Infrastructure.Data;
using HotChocolate;
using HotChocolate.Data;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class KeywordQueries
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Keyword> GetKeywords(
        [Service] ApplicationDbContext context,
        string? searchTerm = null)
    {
        var query = context.Keywords.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(k => k.Name.Contains(searchTerm));
        }

        return query;
    }

    [UseProjection]
    public async Task<Keyword?> GetKeywordById(
        int id,
        [Service] ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        return await context.Keywords.FindAsync(new object[] { id }, cancellationToken);
    }

    [UseProjection]
    public async Task<Keyword?> GetKeywordByTmdbId(
        int tmdbId,
        [Service] ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        return await Task.FromResult(context.Keywords.FirstOrDefault(k => k.TmdbId == tmdbId));
    }
}
