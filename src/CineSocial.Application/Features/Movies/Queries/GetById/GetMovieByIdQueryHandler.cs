using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Movies.Queries.GetById;

public class GetMovieByIdQueryHandler : IRequestHandler<GetMovieByIdQuery, Result<MovieDetailDto>>
{
    private readonly IRepository<MovieEntity> _movieRepository;

    public GetMovieByIdQueryHandler(IRepository<MovieEntity> movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<Result<MovieDetailDto>> Handle(GetMovieByIdQuery request, CancellationToken cancellationToken)
    {
        var movie = await _movieRepository.GetQueryable()
            .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
            .Include(m => m.MovieCasts.OrderBy(mc => mc.CastOrder).Take(10))
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
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (movie == null)
        {
            return Result<MovieDetailDto>.Failure("Movie not found");
        }

        var movieDetail = new MovieDetailDto
        {
            Id = movie.Id,
            TmdbId = movie.TmdbId,
            Title = movie.Title,
            OriginalTitle = movie.OriginalTitle,
            Overview = movie.Overview,
            ReleaseDate = movie.ReleaseDate,
            Runtime = movie.Runtime,
            Budget = movie.Budget,
            Revenue = movie.Revenue,
            PosterPath = movie.PosterPath,
            BackdropPath = movie.BackdropPath,
            ImdbId = movie.ImdbId,
            OriginalLanguage = movie.OriginalLanguage,
            Popularity = movie.Popularity,
            VoteAverage = movie.VoteAverage,
            VoteCount = movie.VoteCount,
            Status = movie.Status,
            Tagline = movie.Tagline,
            Homepage = movie.Homepage,
            Adult = movie.Adult,

            Genres = movie.MovieGenres
                .Select(mg => new GenreDto(mg.Genre.Id, mg.Genre.Name))
                .ToList(),

            Cast = movie.MovieCasts
                .OrderBy(mc => mc.CastOrder)
                .Select(mc => new CastDto(
                    mc.Person.Id,
                    mc.Person.Name,
                    mc.Character,
                    mc.CastOrder,
                    mc.Person.ProfilePath
                ))
                .ToList(),

            Crew = movie.MovieCrews
                .Select(mc => new CrewDto(
                    mc.Person.Id,
                    mc.Person.Name,
                    mc.Job,
                    mc.Department,
                    mc.Person.ProfilePath
                ))
                .ToList(),

            ProductionCompanies = movie.MovieProductionCompanies
                .Select(mpc => new ProductionCompanyDto(
                    mpc.ProductionCompany.Id,
                    mpc.ProductionCompany.Name,
                    mpc.ProductionCompany.LogoPath,
                    mpc.ProductionCompany.OriginCountry
                ))
                .ToList(),

            Countries = movie.MovieCountries
                .Select(mc => mc.Country.Name)
                .ToList(),

            Languages = movie.MovieLanguages
                .Select(ml => ml.Language.Name)
                .ToList(),

            Keywords = movie.MovieKeywords
                .Select(mk => mk.Keyword.Name)
                .ToList(),

            Videos = movie.MovieVideos
                .Select(mv => new VideoDto(
                    mv.VideoKey,
                    mv.Name,
                    mv.Site,
                    mv.Type,
                    mv.Official
                ))
                .ToList(),

            Images = movie.MovieImages
                .Select(mi => new ImageDto(
                    mi.FilePath,
                    mi.ImageType,
                    mi.Width,
                    mi.Height
                ))
                .ToList()
        };

        return Result<MovieDetailDto>.Success(movieDetail);
    }
}
