using CineSocial.Domain.Entities.Movie;
using CineSocial.Infrastructure.Data;
using HotChocolate;
using HotChocolate.Data;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class CountryQueries
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Country> GetCountries(
        [Service] ApplicationDbContext context)
    {
        return context.Countries;
    }

    [UseProjection]
    public async Task<Country?> GetCountryById(
        int id,
        [Service] ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        return await context.Countries.FindAsync(new object[] { id }, cancellationToken);
    }

    [UseProjection]
    public async Task<Country?> GetCountryByIso(
        string iso31661,
        [Service] ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        return await Task.FromResult(context.Countries.FirstOrDefault(c => c.Iso31661 == iso31661));
    }
}
