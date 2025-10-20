using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Rates.Queries.GetMovieRatingStats;

public class GetMovieRatingStatsQueryHandler : IRequestHandler<GetMovieRatingStatsQuery, Result<MovieRatingStatsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMovieRatingStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Result<MovieRatingStatsDto>> Handle(GetMovieRatingStatsQuery request, CancellationToken cancellationToken)
    {
        var ratings = _context.Rates
            .Where(r => r.MovieId == request.MovieId)
            .ToList();

        var stats = new MovieRatingStatsDto
        {
            AverageRating = ratings.Any() ? ratings.Average(r => r.Rating) : 0,
            TotalRatings = ratings.Count
        };

        return Task.FromResult(Result<MovieRatingStatsDto>.Success(stats));
    }
}
