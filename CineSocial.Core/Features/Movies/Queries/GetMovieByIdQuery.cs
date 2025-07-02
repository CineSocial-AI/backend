using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Movies.Queries;

public record GetMovieByIdQuery(Guid Id) : IRequest<Result<MovieDetailResult>>;

public record MovieDetailResult(
    Guid Id,
    string Title,
    string OriginalTitle,
    string Overview,
    DateTime ReleaseDate,
    int Runtime,
    decimal VoteAverage,
    int VoteCount,
    string Language,
    decimal Popularity,
    string Status,
    long Budget,
    long Revenue,
    string? Tagline,
    List<GenreResult> Genres,
    List<MovieCastResult> Cast,
    List<MovieCrewResult> Crew
);

public record MovieCastResult(
    Guid PersonId,
    string Name,
    string Character,
    int Order
);

public record MovieCrewResult(
    Guid PersonId,
    string Name,
    string Job,
    string Department
);

public class GetMovieByIdQueryHandler : IRequestHandler<GetMovieByIdQuery, Result<MovieDetailResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMovieByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MovieDetailResult>> Handle(GetMovieByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get the movie with basic navigation properties first
            var movie = await _unitOfWork.Movies.GetByIdAsync(
                request.Id,
                m => m.MovieGenres!,
                m => m.MovieCasts!,
                m => m.MovieCrews!
            );

            if (movie == null)
            {
                return Result<MovieDetailResult>.Failure("Film bulunamadı.");
            }

            // Load related entities separately due to EF Include limitations
            var genres = new List<GenreResult>();
            if (movie.MovieGenres != null)
            {
                foreach (var movieGenre in movie.MovieGenres)
                {
                    var genre = await _unitOfWork.Genres.GetByIdAsync(movieGenre.GenreId, cancellationToken);
                    if (genre != null)
                    {
                        genres.Add(new GenreResult(genre.Id, genre.Name, genre.Description ?? ""));
                    }
                }
            }

            var cast = new List<MovieCastResult>();
            if (movie.MovieCasts != null)
            {
                foreach (var movieCast in movie.MovieCasts)
                {
                    var person = await _unitOfWork.Persons.GetByIdAsync(movieCast.PersonId, cancellationToken);
                    if (person != null)
                    {
                        cast.Add(new MovieCastResult(person.Id, person.Name, movieCast.Character, movieCast.Order));
                    }
                }
            }

            var crew = new List<MovieCrewResult>();
            if (movie.MovieCrews != null)
            {
                foreach (var movieCrew in movie.MovieCrews)
                {
                    var person = await _unitOfWork.Persons.GetByIdAsync(movieCrew.PersonId, cancellationToken);
                    if (person != null)
                    {
                        crew.Add(new MovieCrewResult(person.Id, person.Name, movieCrew.Job, movieCrew.Department));
                    }
                }
            }

            var result = new MovieDetailResult(
                movie.Id,
                movie.Title,
                movie.OriginalTitle ?? "",
                movie.Overview ?? "",
                movie.ReleaseDate ?? DateTime.MinValue,
                movie.Runtime ?? 0,
                movie.VoteAverage ?? 0,
                movie.VoteCount ?? 0,
                movie.Language ?? "",
                movie.Popularity ?? 0,
                movie.Status ?? "",
                movie.Budget ?? 0,
                movie.Revenue ?? 0,
                movie.Tagline,
                genres,
                cast.OrderBy(c => c.Order).ToList(),
                crew
            );

            return Result<MovieDetailResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<MovieDetailResult>.Failure($"Film detayları yüklenirken hata oluştu: {ex.Message}");
        }
    }
}