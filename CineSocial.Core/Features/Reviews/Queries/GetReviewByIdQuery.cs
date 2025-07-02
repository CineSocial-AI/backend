using CineSocial.Core.Features.Reviews.Commands;
using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Reviews.Queries;

public record GetReviewByIdQuery(Guid Id) : IRequest<Result<ReviewResult?>>;

public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, Result<ReviewResult?>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetReviewByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReviewResult?>> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var review = await _unitOfWork.Reviews.FirstOrDefaultAsync(
                r => r.Id == request.Id,
                r => r.User!
            );

            if (review == null)
            {
                return Result<ReviewResult?>.Success(null);
            }

            var result = new ReviewResult(
                review.Id,
                review.UserId,
                review.MovieId,
                review.Title,
                review.Content,
                review.ContainsSpoilers,
                review.LikesCount,
                review.CreatedAt,
                review.UpdatedAt,
                $"{review.User?.FirstName} {review.User?.LastName}",
                review.User?.Username ?? ""
            );

            return Result<ReviewResult?>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<ReviewResult?>.Failure($"Değerlendirme sorgulanırken hata oluştu: {ex.Message}");
        }
    }
}