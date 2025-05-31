using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.DTOs.Posts;

namespace CineSocial.Core.Application.Ports;

public interface IPostService
{
    Task<Result<PagedResult<PostSummaryDto>>> GetPostsAsync(int page = 1, int pageSize = 20, Guid? groupId = null, Guid? userId = null, string? search = null, string? sortBy = null);
    Task<Result<PostDto>> GetPostByIdAsync(Guid id, Guid? currentUserId = null);
    Task<Result<PostDto>> CreatePostAsync(Guid userId, CreatePostDto createDto);
    Task<Result<PostDto>> UpdatePostAsync(Guid userId, Guid postId, UpdatePostDto updateDto);
    Task<Result> DeletePostAsync(Guid userId, Guid postId);
    Task<Result> ReactToPostAsync(Guid userId, Guid postId, ReactionType reactionType);
    Task<Result> RemovePostReactionAsync(Guid userId, Guid postId);
    Task<Result<PagedResult<PostCommentDto>>> GetPostCommentsAsync(Guid postId, int page = 1, int pageSize = 20, Guid? currentUserId = null);
    Task<Result<PostCommentDto>> CreateCommentAsync(Guid userId, CreatePostCommentDto createDto);
    Task<Result<PostCommentDto>> UpdateCommentAsync(Guid userId, Guid commentId, UpdatePostCommentDto updateDto);
    Task<Result> DeleteCommentAsync(Guid userId, Guid commentId);
    Task<Result> ReactToCommentAsync(Guid userId, Guid commentId, ReactionType reactionType);
    Task<Result> RemoveCommentReactionAsync(Guid userId, Guid commentId);
    Task<Result<List<PostSummaryDto>>> GetTrendingPostsAsync(int count = 10);
    Task<Result<List<PostSummaryDto>>> GetUserFeedAsync(Guid userId, int page = 1, int pageSize = 20);
}

