using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Movie;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CineSocial.Application.UseCases.Movies;

public class GetMovieByIdUseCase
{
    private readonly IRepository<MovieEntity> _movieRepository;
    private readonly ILogger<GetMovieByIdUseCase> _logger;

    public GetMovieByIdUseCase(
        IRepository<MovieEntity> movieRepository,
        ILogger<GetMovieByIdUseCase> logger)
    {
        _movieRepository = movieRepository;
        _logger = logger;
    }

    public async Task<MovieEntity?> ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching movie by ID: MovieId={MovieId}", id);

        var movie = await _movieRepository.GetQueryable()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (movie == null)
        {
            _logger.LogWarning("Movie not found: MovieId={MovieId}", id);
        }
        else
        {
            _logger.LogInformation("Movie fetched successfully: MovieId={MovieId}, Title={Title}", id, movie.Title);
        }

        return movie;
    }
}
