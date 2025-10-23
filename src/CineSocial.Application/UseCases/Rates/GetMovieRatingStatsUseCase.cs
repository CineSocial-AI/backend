using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Features.Rates.Queries.GetMovieRatingStats;

namespace CineSocial.Application.UseCases.Rates;

public class GetMovieRatingStatsUseCase
{
    private readonly IApplicationDbContext _context;

    public GetMovieRatingStatsUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MovieRatingStatsDto> ExecuteAsync(int movieId, CancellationToken cancellationToken = default)
    {
        var ratings = _context.Rates
            .Where(r => r.MovieId == movieId)
            .ToList();

        return new MovieRatingStatsDto
        {
            AverageRating = ratings.Any() ? ratings.Average(r => r.Rating) : 0,
            TotalRatings = ratings.Count
        };
    }
}
