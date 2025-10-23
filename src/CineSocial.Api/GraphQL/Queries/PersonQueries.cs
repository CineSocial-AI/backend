using CineSocial.Domain.Entities.Movie;
using CineSocial.Infrastructure.Data;
using HotChocolate;
using HotChocolate.Data;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class PersonQueries
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Person> GetPersons(
        [Service] ApplicationDbContext context,
        string? searchTerm = null)
    {
        var query = context.People.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm));
        }

        return query;
    }

    [UseProjection]
    public async Task<Person?> GetPersonById(
        int id,
        [Service] ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        return await context.People.FindAsync(new object[] { id }, cancellationToken);
    }

    [UseProjection]
    public async Task<Person?> GetPersonByTmdbId(
        int tmdbId,
        [Service] ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        return await Task.FromResult(context.People.FirstOrDefault(p => p.TmdbId == tmdbId));
    }
}
