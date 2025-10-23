using CineSocial.Domain.Entities.Movie;
using CineSocial.Infrastructure.Data;
using HotChocolate;
using HotChocolate.Data;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class ProductionCompanyQueries
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<ProductionCompany> GetProductionCompanies(
        [Service] ApplicationDbContext context,
        string? searchTerm = null)
    {
        var query = context.ProductionCompanies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(pc => pc.Name.Contains(searchTerm));
        }

        return query;
    }

    [UseProjection]
    public async Task<ProductionCompany?> GetProductionCompanyById(
        int id,
        [Service] ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        return await context.ProductionCompanies.FindAsync(new object[] { id }, cancellationToken);
    }

    [UseProjection]
    public async Task<ProductionCompany?> GetProductionCompanyByTmdbId(
        int tmdbId,
        [Service] ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        return await Task.FromResult(context.ProductionCompanies.FirstOrDefault(pc => pc.TmdbId == tmdbId));
    }
}
