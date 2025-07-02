using CineSocial.Core.Features.Ratings.Commands;
using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Ratings.Queries;

public record GetMovieRatingsQuery(Guid MovieId) : IRequest<Result<MovieRatingStats>>;

public record MovieRatingStats(
    Guid MovieId,
    decimal AverageRating,
    int TotalRatings,
    Dictionary<int, int> RatingDistribution
);

public class GetMovieRatingsQueryHandler : IRequestHandler<GetMovieRatingsQuery, Result<MovieRatingStats>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMovieRatingsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MovieRatingStats>> Handle(GetMovieRatingsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get ratings for the movie (don't check if movie exists - ratings might exist for deleted movies)
            var ratings = await _unitOfWork.Ratings.FindAsync(
                r => r.MovieId == request.MovieId,
                cancellationToken
            );

            var ratingsList = ratings.ToList();

            if (!ratingsList.Any())
            {
                var emptyStats = new MovieRatingStats(
                    request.MovieId,
                    0,
                    0,
                    new Dictionary<int, int>()
                );
                return Result<MovieRatingStats>.Success(emptyStats);
            }

            var averageRating = ratingsList.Average(r => r.Score);
            var totalRatings = ratingsList.Count;

            // Rating distribution (1-10)
            var distribution = new Dictionary<int, int>();
            for (int i = 1; i <= 10; i++)
            {
                distribution[i] = ratingsList.Count(r => r.Score == i);
            }

            var stats = new MovieRatingStats(
                request.MovieId,
                Math.Round((decimal)averageRating, 2),
                totalRatings,
                distribution
            );

            return Result<MovieRatingStats>.Success(stats);
        }
        catch (Exception ex)
        {
            return Result<MovieRatingStats>.Failure($"Film puanları sorgulanırken hata oluştu: {ex.Message}");
        }
    }
}