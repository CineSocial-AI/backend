using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.DTOs.Reviews;

namespace CineSocial.Core.Application.Ports;

public interface IReviewService
{
    Task<Result<PagedResult<ReviewDto>>> GetReviewsAsync(int page = 1, int pageSize = 20, Guid? movieId = null, Guid? userId = null);
    Task<Result<ReviewDto>> GetReviewByIdAsync(Guid id, Guid? currentUserId = null);
    Task<Result<ReviewDto>> CreateReviewAsync(Guid userId, CreateReviewDto createDto);
    Task<Result<ReviewDto>> UpdateReviewAsync(Guid userId, Guid reviewId, UpdateReviewDto updateDto);
    Task<Result> DeleteReviewAsync(Guid userId, Guid reviewId);
    Task<Result> LikeReviewAsync(Guid userId, Guid reviewId, bool isLike);
    Task<Result> RemoveLikeAsync(Guid userId, Guid reviewId);
    Task<Result<PagedResult<CommentDto>>> GetCommentsAsync(Guid reviewId, int page = 1, int pageSize = 20, Guid? currentUserId = null);
    Task<Result<CommentDto>> CreateCommentAsync(Guid userId, CreateCommentDto createDto);
    Task<Result<CommentDto>> UpdateCommentAsync(Guid userId, Guid commentId, UpdateCommentDto updateDto);
    Task<Result> DeleteCommentAsync(Guid userId, Guid commentId);
    Task<Result> LikeCommentAsync(Guid userId, Guid commentId, bool isLike);
    Task<Result> RemoveCommentLikeAsync(Guid userId, Guid commentId);
}

