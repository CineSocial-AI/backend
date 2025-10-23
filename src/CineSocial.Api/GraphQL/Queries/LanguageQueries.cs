using CineSocial.Domain.Entities.Movie;
using CineSocial.Infrastructure.Data;
using HotChocolate;
using HotChocolate.Data;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class LanguageQueries
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Language> GetLanguages(
        [Service] ApplicationDbContext context)
    {
        return context.Languages;
    }

    [UseProjection]
    public async Task<Language?> GetLanguageById(
        int id,
        [Service] ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        return await context.Languages.FindAsync(new object[] { id }, cancellationToken);
    }

    [UseProjection]
    public async Task<Language?> GetLanguageByIso(
        string iso6391,
        [Service] ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        return await Task.FromResult(context.Languages.FirstOrDefault(l => l.Iso6391 == iso6391));
    }
}
