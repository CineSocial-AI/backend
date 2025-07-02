using CineSocial.Core.Features.Ratings.Commands;
using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Ratings.Queries;

public record GetUserRatingQuery(
    Guid UserId,
    Guid MovieId
) : IRequest<Result<RatingResult?>>;

public class GetUserRatingQueryHandler : IRequestHandler<GetUserRatingQuery, Result<RatingResult?>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserRatingQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RatingResult?>> Handle(GetUserRatingQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var rating = await _unitOfWork.Ratings.FirstOrDefaultAsync(
                r => r.UserId == request.UserId && r.MovieId == request.MovieId,
                cancellationToken
            );

            if (rating == null)
            {
                return Result<RatingResult?>.Success(null);
            }

            var result = new RatingResult(
                rating.Id,
                rating.UserId,
                rating.MovieId,
                rating.Score,
                rating.CreatedAt,
                rating.UpdatedAt
            );

            return Result<RatingResult?>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<RatingResult?>.Failure($"Puanlama sorgulanırken hata oluştu: {ex.Message}");
        }
    }
}