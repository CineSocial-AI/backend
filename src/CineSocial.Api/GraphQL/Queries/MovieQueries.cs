using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Movie;
using HotChocolate;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class MovieQueries
{
    /// <summary>
    /// Get movies for list view (homepage) - lightweight
    /// </summary>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<MovieEntity> GetMovies(
        [Service] IRepository<MovieEntity> repository)
    {
        return repository.GetQueryable()
            .OrderByDescending(m => m.Popularity);
    }

    /// <summary>
    /// Get single movie with full details for detail page
    /// </summary>
    public async Task<MovieEntity?> GetMovie(
        int id,
        [Service] IRepository<MovieEntity> repository,
        CancellationToken cancellationToken)
    {
        return await repository.GetQueryable()
            .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
            .Include(m => m.MovieCasts)
                .ThenInclude(mc => mc.Person)
            .Include(m => m.MovieCrews)
                .ThenInclude(mc => mc.Person)
            .Include(m => m.MovieProductionCompanies)
                .ThenInclude(mpc => mpc.ProductionCompany)
            .Include(m => m.MovieCountries)
                .ThenInclude(mc => mc.Country)
            .Include(m => m.MovieLanguages)
                .ThenInclude(ml => ml.Language)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    /// <summary>
    /// Search movies by title
    /// </summary>
    [UseProjection]
    public IQueryable<MovieEntity> SearchMovies(
        string searchTerm,
        [Service] IRepository<MovieEntity> repository)
    {
        return repository.GetQueryable()
            .Where(m => m.Title.Contains(searchTerm) ||
                       (m.OriginalTitle != null && m.OriginalTitle.Contains(searchTerm)))
            .OrderByDescending(m => m.Popularity)
            .Take(20);
    }
}
