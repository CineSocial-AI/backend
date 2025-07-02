using CineSocial.Core.Features.Comments.Commands;
using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Comments.Queries;

public record GetReviewCommentsQuery(
    Guid ReviewId,
    int Page = 1,
    int PageSize = 10,
    string SortBy = "created_at",
    string SortOrder = "asc"
) : IRequest<Result<CommentListResult>>;

public record CommentListResult(
    List<CommentWithRepliesResult> Comments,
    int TotalCount,
    int TotalPages,
    int CurrentPage,
    int PageSize
);

public record CommentWithRepliesResult(
    Guid Id,
    Guid UserId,
    Guid ReviewId,
    string Content,
    int UpvotesCount,
    int DownvotesCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    Guid? ParentCommentId,
    string UserFullName,
    string UserUsername,
    List<CommentResult> Replies
);

public class GetReviewCommentsQueryHandler : IRequestHandler<GetReviewCommentsQuery, Result<CommentListResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetReviewCommentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CommentListResult>> Handle(GetReviewCommentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if review exists
            var review = await _unitOfWork.Reviews.GetByIdAsync(request.ReviewId, cancellationToken);
            if (review == null)
            {
                return Result<CommentListResult>.Failure("Değerlendirme bulunamadı.");
            }

            // Get all comments for the review with user information
            var allComments = await _unitOfWork.Comments.FindAsync(
                c => c.ReviewId == request.ReviewId,
                c => c.User!
            );

            var commentsList = allComments.ToList();

            // Separate parent comments (top-level) and replies
            var parentComments = commentsList
                .Where(c => c.ParentCommentId == null)
                .ToList();

            var replies = commentsList
                .Where(c => c.ParentCommentId != null)
                .ToList();

            // Apply sorting to parent comments
            parentComments = request.SortBy.ToLower() switch
            {
                "upvotes" => request.SortOrder.ToLower() == "asc" 
                    ? parentComments.OrderBy(c => c.UpvotesCount).ToList()
                    : parentComments.OrderByDescending(c => c.UpvotesCount).ToList(),
                "updated_at" => request.SortOrder.ToLower() == "asc"
                    ? parentComments.OrderBy(c => c.UpdatedAt ?? c.CreatedAt).ToList()
                    : parentComments.OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt).ToList(),
                _ => request.SortOrder.ToLower() == "asc"
                    ? parentComments.OrderBy(c => c.CreatedAt).ToList()
                    : parentComments.OrderByDescending(c => c.CreatedAt).ToList()
            };

            // Apply pagination to parent comments
            var totalCount = parentComments.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var skip = (request.Page - 1) * request.PageSize;
            var pagedParentComments = parentComments.Skip(skip).Take(request.PageSize).ToList();

            // Build comment tree with replies
            var commentsWithReplies = pagedParentComments.Select(comment =>
            {
                var commentReplies = replies
                    .Where(r => r.ParentCommentId == comment.Id)
                    .OrderBy(r => r.CreatedAt)
                    .Select(r => new CommentResult(
                        r.Id,
                        r.UserId,
                        r.ReviewId,
                        r.Content,
                        r.UpvotesCount,
                        r.DownvotesCount,
                        r.CreatedAt,
                        r.UpdatedAt,
                        r.ParentCommentId,
                        $"{r.User?.FirstName} {r.User?.LastName}",
                        r.User?.Username ?? ""
                    )).ToList();

                return new CommentWithRepliesResult(
                    comment.Id,
                    comment.UserId,
                    comment.ReviewId,
                    comment.Content,
                    comment.UpvotesCount,
                    comment.DownvotesCount,
                    comment.CreatedAt,
                    comment.UpdatedAt,
                    comment.ParentCommentId,
                    $"{comment.User?.FirstName} {comment.User?.LastName}",
                    comment.User?.Username ?? "",
                    commentReplies
                );
            }).ToList();

            var result = new CommentListResult(
                commentsWithReplies,
                totalCount,
                totalPages,
                request.Page,
                request.PageSize
            );

            return Result<CommentListResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<CommentListResult>.Failure($"Yorumlar sorgulanırken hata oluştu: {ex.Message}");
        }
    }
}