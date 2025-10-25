using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Features.Rates.Queries.GetMovieRatingStats;
using Microsoft.Extensions.Logging;

namespace CineSocial.Application.UseCases.Rates;

public class GetMovieRatingStatsUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetMovieRatingStatsUseCase> _logger;

    public GetMovieRatingStatsUseCase(
        IApplicationDbContext context,
        ILogger<GetMovieRatingStatsUseCase> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MovieRatingStatsDto> ExecuteAsync(int movieId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching rating stats for movie: MovieId={MovieId}", movieId);

        var ratings = _context.Rates
            .Where(r => r.MovieId == movieId)
            .ToList();

        var stats = new MovieRatingStatsDto
        {
            AverageRating = ratings.Any() ? ratings.Average(r => r.Rating) : 0,
            TotalRatings = ratings.Count
        };

        _logger.LogInformation("Rating stats fetched: MovieId={MovieId}, AverageRating={AverageRating}, TotalRatings={TotalRatings}",
            movieId, stats.AverageRating, stats.TotalRatings);

        return stats;
    }
}
