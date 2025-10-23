using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Movie;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.Movies;

public class GetMovieByIdUseCase
{
    private readonly IRepository<MovieEntity> _movieRepository;

    public GetMovieByIdUseCase(IRepository<MovieEntity> movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<MovieEntity?> ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _movieRepository.GetQueryable()
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
            .Include(m => m.MovieKeywords)
                .ThenInclude(mk => mk.Keyword)
            .Include(m => m.MovieVideos)
            .Include(m => m.MovieImages)
            .Include(m => m.MovieCollections)
                .ThenInclude(mc => mc.Collection)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }
}
