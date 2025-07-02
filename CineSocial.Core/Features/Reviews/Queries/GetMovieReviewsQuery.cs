using CineSocial.Core.Features.Reviews.Commands;
using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Reviews.Queries;

public record GetMovieReviewsQuery(
    Guid MovieId,
    int Page = 1,
    int PageSize = 10,
    string SortBy = "created_at",
    string SortOrder = "desc"
) : IRequest<Result<ReviewListResult>>;

public record ReviewListResult(
    List<ReviewResult> Reviews,
    int TotalCount,
    int TotalPages,
    int CurrentPage,
    int PageSize
);

public class GetMovieReviewsQueryHandler : IRequestHandler<GetMovieReviewsQuery, Result<ReviewListResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMovieReviewsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReviewListResult>> Handle(GetMovieReviewsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if movie exists
            var movie = await _unitOfWork.Movies.GetByIdAsync(request.MovieId, cancellationToken);
            if (movie == null)
            {
                return Result<ReviewListResult>.Failure("Film bulunamadı.");
            }

            // Get reviews with user information
            var reviews = await _unitOfWork.Reviews.FindAsync(
                r => r.MovieId == request.MovieId,
                r => r.User!
            );

            var reviewsList = reviews.ToList();

            // Apply sorting
            reviewsList = request.SortBy.ToLower() switch
            {
                "likes_count" => request.SortOrder.ToLower() == "asc" 
                    ? reviewsList.OrderBy(r => r.LikesCount).ToList()
                    : reviewsList.OrderByDescending(r => r.LikesCount).ToList(),
                "updated_at" => request.SortOrder.ToLower() == "asc"
                    ? reviewsList.OrderBy(r => r.UpdatedAt ?? r.CreatedAt).ToList()
                    : reviewsList.OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt).ToList(),
                _ => request.SortOrder.ToLower() == "asc"
                    ? reviewsList.OrderBy(r => r.CreatedAt).ToList()
                    : reviewsList.OrderByDescending(r => r.CreatedAt).ToList()
            };

            // Apply pagination
            var totalCount = reviewsList.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var skip = (request.Page - 1) * request.PageSize;
            var pagedReviews = reviewsList.Skip(skip).Take(request.PageSize).ToList();

            var reviewResults = pagedReviews.Select(r => new ReviewResult(
                r.Id,
                r.UserId,
                r.MovieId,
                r.Title,
                r.Content,
                r.ContainsSpoilers,
                r.LikesCount,
                r.CreatedAt,
                r.UpdatedAt,
                $"{r.User?.FirstName} {r.User?.LastName}",
                r.User?.Username ?? ""
            )).ToList();

            var result = new ReviewListResult(
                reviewResults,
                totalCount,
                totalPages,
                request.Page,
                request.PageSize
            );

            return Result<ReviewListResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<ReviewListResult>.Failure($"Değerlendirmeler sorgulanırken hata oluştu: {ex.Message}");
        }
    }
}