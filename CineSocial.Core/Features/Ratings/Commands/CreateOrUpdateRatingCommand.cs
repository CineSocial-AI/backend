using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using MediatR;

namespace CineSocial.Core.Features.Ratings.Commands;

public record CreateOrUpdateRatingCommand(
    Guid UserId,
    Guid MovieId,
    int Score
) : IRequest<Result<RatingResult>>;

public record RatingResult(
    Guid Id,
    Guid UserId,
    Guid MovieId,
    int Score,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public class CreateOrUpdateRatingCommandHandler : IRequestHandler<CreateOrUpdateRatingCommand, Result<RatingResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrUpdateRatingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RatingResult>> Handle(CreateOrUpdateRatingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if movie exists
            var movie = await _unitOfWork.Movies.GetByIdAsync(request.MovieId, cancellationToken);
            if (movie == null)
            {
                return Result<RatingResult>.Failure("Film bulunamadı.");
            }

            // Check if user already rated this movie
            var existingRating = await _unitOfWork.Ratings.FirstOrDefaultAsync(
                r => r.UserId == request.UserId && r.MovieId == request.MovieId,
                cancellationToken
            );

            Rating rating;

            if (existingRating != null)
            {
                // Update existing rating
                existingRating.Score = request.Score;
                existingRating.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Ratings.Update(existingRating);
                rating = existingRating;
            }
            else
            {
                // Create new rating
                rating = new Rating
                {
                    UserId = request.UserId,
                    MovieId = request.MovieId,
                    Score = request.Score,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Ratings.AddAsync(rating, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new RatingResult(
                rating.Id,
                rating.UserId,
                rating.MovieId,
                rating.Score,
                rating.CreatedAt,
                rating.UpdatedAt
            );

            return Result<RatingResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<RatingResult>.Failure($"Puanlama kaydedilirken hata oluştu: {ex.Message}");
        }
    }
}