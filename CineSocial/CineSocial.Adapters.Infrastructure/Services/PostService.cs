using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CineSocial.Core.Application.Ports;
using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.DTOs.Posts;
using CineSocial.Core.Domain.Entities;
using CineSocial.Adapters.Infrastructure.Database;
// Enum aliasing to resolve ambiguity
using DomainReactionType = CineSocial.Core.Domain.Entities.ReactionType;
using DtoReactionType = CineSocial.Core.Application.DTOs.Posts.ReactionType;

namespace CineSocial.Adapters.Infrastructure.Services;

public class PostService : IPostService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PostService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<PostSummaryDto>>> GetPostsAsync(int page = 1, int pageSize = 20, Guid? groupId = null, Guid? userId = null, string? search = null, string? sortBy = null)
    {
        try
        {
            var query = _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Group)
                .Include(p => p.Media)
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            if (groupId.HasValue)
                query = query.Where(p => p.GroupId == groupId.Value);

            if (userId.HasValue)
                query = query.Where(p => p.AuthorId == userId.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Title.Contains(search) || p.Content.Contains(search));

            query = sortBy?.ToLower() switch
            {
                "hot" => query.OrderByDescending(p => p.UpvoteCount - p.DownvoteCount).ThenByDescending(p => p.CreatedAt),
                "new" => query.OrderByDescending(p => p.CreatedAt),
                "top" => query.OrderByDescending(p => p.UpvoteCount),
                _ => query.OrderByDescending(p => (p.UpvoteCount - p.DownvoteCount) * 1.0 / Math.Max(1, (DateTime.UtcNow - p.CreatedAt).TotalHours))
            };

            var totalCount = await query.CountAsync();
            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var postDtos = _mapper.Map<List<PostSummaryDto>>(posts);
            var result = new PagedResult<PostSummaryDto>(postDtos, totalCount, page, pageSize);
            return Result<PagedResult<PostSummaryDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<PostSummaryDto>>.Failure($"Error getting posts: {ex.Message}");
        }
    }

    public async Task<Result<PostDto>> GetPostByIdAsync(Guid id, Guid? currentUserId = null)
    {
        try
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Group)
                .Include(p => p.Media)
                .Include(p => p.Tags)
                .Include(p => p.Reactions)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (post == null)
                return Result<PostDto>.Failure("Post not found");

            post.ViewCount++;
            await _context.SaveChangesAsync();

            var postDto = _mapper.Map<PostDto>(post);

            if (currentUserId.HasValue)
            {
                var userReaction = post.Reactions.FirstOrDefault(r => r.UserId == currentUserId.Value);
                postDto.CurrentUserReaction = userReaction != null ? (DtoReactionType?)userReaction.Type : null;
            }

            return Result<PostDto>.Success(postDto);
        }
        catch (Exception ex)
        {
            return Result<PostDto>.Failure($"Error getting post: {ex.Message}");
        }
    }

    public async Task<Result<PostDto>> CreatePostAsync(Guid userId, CreatePostDto createDto)
    {
        try
        {
            var isMember = await _context.GroupMembers
                .AnyAsync(m => m.GroupId == createDto.GroupId && m.UserId == userId && m.IsActive);

            if (!isMember)
                return Result<PostDto>.Failure("You must be a member of the group to post");

            var post = _mapper.Map<Post>(createDto);
            post.Id = Guid.NewGuid();
            post.AuthorId = userId;
            post.CreatedAt = DateTime.UtcNow;

            _context.Posts.Add(post);

            foreach (var tag in createDto.Tags)
            {
                var postTag = new PostTag
                {
                    Id = Guid.NewGuid(),
                    PostId = post.Id,
                    Tag = tag.Trim().ToLower(),
                    CreatedAt = DateTime.UtcNow
                };
                _context.PostTags.Add(postTag);
            }

            await _context.SaveChangesAsync();
            return await GetPostByIdAsync(post.Id, userId);
        }
        catch (Exception ex)
        {
            return Result<PostDto>.Failure($"Error creating post: {ex.Message}");
        }
    }

    public async Task<Result<PostDto>> UpdatePostAsync(Guid userId, Guid postId, UpdatePostDto updateDto)
    {
        try
        {
            var post = await _context.Posts
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == postId && p.AuthorId == userId && !p.IsDeleted);

            if (post == null)
                return Result<PostDto>.Failure("Post not found");

            _mapper.Map(updateDto, post);
            post.UpdatedAt = DateTime.UtcNow;

            _context.PostTags.RemoveRange(post.Tags);
            foreach (var tag in updateDto.Tags)
            {
                var postTag = new PostTag
                {
                    Id = Guid.NewGuid(),
                    PostId = post.Id,
                    Tag = tag.Trim().ToLower(),
                    CreatedAt = DateTime.UtcNow
                };
                _context.PostTags.Add(postTag);
            }

            await _context.SaveChangesAsync();
            return await GetPostByIdAsync(postId, userId);
        }
        catch (Exception ex)
        {
            return Result<PostDto>.Failure($"Error updating post: {ex.Message}");
        }
    }

    public async Task<Result> DeletePostAsync(Guid userId, Guid postId)
    {
        try
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == postId && p.AuthorId == userId);

            if (post == null)
                return Result.Failure("Post not found");

            post.IsDeleted = true;
            post.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting post: {ex.Message}");
        }
    }

    public async Task<Result> ReactToPostAsync(Guid userId, Guid postId, DtoReactionType reactionType)
    {
        try
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
                return Result.Failure("Post not found");

            var existingReaction = await _context.PostReactions
                .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);

            if (existingReaction != null)
            {
                if ((DtoReactionType)existingReaction.Type == reactionType)
                    return Result.Failure("You already reacted with this type");

                if (existingReaction.Type == DomainReactionType.Upvote)
                    post.UpvoteCount--;
                else
                    post.DownvoteCount--;

                existingReaction.Type = (DomainReactionType)reactionType;
            }
            else
            {
                var reaction = new PostReaction
                {
                    Id = Guid.NewGuid(),
                    PostId = postId,
                    UserId = userId,
                    Type = (DomainReactionType)reactionType,
                    CreatedAt = DateTime.UtcNow
                };
                _context.PostReactions.Add(reaction);
            }

            if (reactionType == DtoReactionType.Upvote)
                post.UpvoteCount++;
            else
                post.DownvoteCount++;

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error reacting to post: {ex.Message}");
        }
    }

    public async Task<Result> RemovePostReactionAsync(Guid userId, Guid postId)
    {
        try
        {
            var reaction = await _context.PostReactions
                .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);

            if (reaction == null)
                return Result.Failure("No reaction found");

            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                if (reaction.Type == DomainReactionType.Upvote)
                    post.UpvoteCount--;
                else
                    post.DownvoteCount--;
            }

            _context.PostReactions.Remove(reaction);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error removing reaction: {ex.Message}");
        }
    }

    public async Task<Result<PagedResult<PostCommentDto>>> GetPostCommentsAsync(Guid postId, int page = 1, int pageSize = 20, Guid? currentUserId = null)
    {
        try
        {
            var query = _context.PostComments
                .Include(c => c.Author)
                .Include(c => c.Replies)
                .ThenInclude(r => r.Author)
                .Where(c => c.PostId == postId && c.ParentCommentId == null && !c.IsDeleted)
                .OrderByDescending(c => c.UpvoteCount - c.DownvoteCount)
                .ThenBy(c => c.CreatedAt);

            var totalCount = await query.CountAsync();
            var comments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var commentDtos = _mapper.Map<List<PostCommentDto>>(comments);

            if (currentUserId.HasValue)
            {
                var allCommentIds = comments.SelectMany(c => new[] { c.Id }.Concat(c.Replies.Select(r => r.Id))).ToList();
                var userReactions = await _context.CommentReactions
                    .Where(r => allCommentIds.Contains(r.CommentId) && r.UserId == currentUserId.Value)
                    .ToListAsync();

                foreach (var commentDto in commentDtos)
                {
                    var reaction = userReactions.FirstOrDefault(r => r.CommentId == commentDto.Id);
                    commentDto.CurrentUserReaction = reaction != null ? (DtoReactionType?)reaction.Type : null;

                    foreach (var reply in commentDto.Replies)
                    {
                        var replyReaction = userReactions.FirstOrDefault(r => r.CommentId == reply.Id);
                        reply.CurrentUserReaction = replyReaction != null ? (DtoReactionType?)replyReaction.Type : null;
                    }
                }
            }

            var result = new PagedResult<PostCommentDto>(commentDtos, totalCount, page, pageSize);
            return Result<PagedResult<PostCommentDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<PostCommentDto>>.Failure($"Error getting comments: {ex.Message}");
        }
    }

    public async Task<Result<PostCommentDto>> CreateCommentAsync(Guid userId, CreatePostCommentDto createDto)
    {
        try
        {
            var comment = _mapper.Map<PostComment>(createDto);
            comment.Id = Guid.NewGuid();
            comment.AuthorId = userId;
            comment.CreatedAt = DateTime.UtcNow;

            _context.PostComments.Add(comment);

            var post = await _context.Posts.FindAsync(createDto.PostId);
            if (post != null)
            {
                post.CommentCount++;
            }

            if (createDto.ParentCommentId.HasValue)
            {
                var parentComment = await _context.PostComments.FindAsync(createDto.ParentCommentId.Value);
                if (parentComment != null)
                {
                    parentComment.ReplyCount++;
                }
            }

            await _context.SaveChangesAsync();

            var createdComment = await _context.PostComments
                .Include(c => c.Author)
                .Include(c => c.Replies)
                .ThenInclude(r => r.Author)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            var commentDto = _mapper.Map<PostCommentDto>(createdComment);
            return Result<PostCommentDto>.Success(commentDto);
        }
        catch (Exception ex)
        {
            return Result<PostCommentDto>.Failure($"Error creating comment: {ex.Message}");
        }
    }

    public async Task<Result<PostCommentDto>> UpdateCommentAsync(Guid userId, Guid commentId, UpdatePostCommentDto updateDto)
    {
        try
        {
            var comment = await _context.PostComments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.AuthorId == userId && !c.IsDeleted);

            if (comment == null)
                return Result<PostCommentDto>.Failure("Comment not found");

            _mapper.Map(updateDto, comment);
            comment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var updatedComment = await _context.PostComments
                .Include(c => c.Author)
                .Include(c => c.Replies)
                .ThenInclude(r => r.Author)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            var commentDto = _mapper.Map<PostCommentDto>(updatedComment);
            return Result<PostCommentDto>.Success(commentDto);
        }
        catch (Exception ex)
        {
            return Result<PostCommentDto>.Failure($"Error updating comment: {ex.Message}");
        }
    }

    public async Task<Result> DeleteCommentAsync(Guid userId, Guid commentId)
    {
        try
        {
            var comment = await _context.PostComments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.AuthorId == userId);

            if (comment == null)
                return Result.Failure("Comment not found");

            comment.IsDeleted = true;
            comment.UpdatedAt = DateTime.UtcNow;

            var post = await _context.Posts.FindAsync(comment.PostId);
            if (post != null)
            {
                post.CommentCount--;
            }

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting comment: {ex.Message}");
        }
    }

    public async Task<Result> ReactToCommentAsync(Guid userId, Guid commentId, DtoReactionType reactionType)
    {
        try
        {
            var comment = await _context.PostComments.FindAsync(commentId);
            if (comment == null)
                return Result.Failure("Comment not found");

            var existingReaction = await _context.CommentReactions
                .FirstOrDefaultAsync(r => r.CommentId == commentId && r.UserId == userId);

            if (existingReaction != null)
            {
                if ((DtoReactionType)existingReaction.Type == reactionType)
                    return Result.Failure("You already reacted with this type");

                if (existingReaction.Type == DomainReactionType.Upvote)
                    comment.UpvoteCount--;
                else
                    comment.DownvoteCount--;

                existingReaction.Type = (DomainReactionType)reactionType;
            }
            else
            {
                var reaction = new CommentReaction
                {
                    Id = Guid.NewGuid(),
                    CommentId = commentId,
                    UserId = userId,
                    Type = (DomainReactionType)reactionType,
                    CreatedAt = DateTime.UtcNow
                };
                _context.CommentReactions.Add(reaction);
            }

            if (reactionType == DtoReactionType.Upvote)
                comment.UpvoteCount++;
            else
                comment.DownvoteCount++;

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error reacting to comment: {ex.Message}");
        }
    }

    public async Task<Result> RemoveCommentReactionAsync(Guid userId, Guid commentId)
    {
        try
        {
            var reaction = await _context.CommentReactions
                .FirstOrDefaultAsync(r => r.CommentId == commentId && r.UserId == userId);

            if (reaction == null)
                return Result.Failure("No reaction found");

            var comment = await _context.PostComments.FindAsync(commentId);
            if (comment != null)
            {
                if (reaction.Type == DomainReactionType.Upvote)
                    comment.UpvoteCount--;
                else
                    comment.DownvoteCount--;
            }

            _context.CommentReactions.Remove(reaction);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error removing comment reaction: {ex.Message}");
        }
    }

    public async Task<Result<List<PostSummaryDto>>> GetTrendingPostsAsync(int count = 10)
    {
        try
        {
            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Group)
                .Include(p => p.Media)
                .Where(p => !p.IsDeleted && p.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                .OrderByDescending(p => (p.UpvoteCount - p.DownvoteCount) * 1.0 / Math.Max(1, (DateTime.UtcNow - p.CreatedAt).TotalHours))
                .Take(count)
                .ToListAsync();

            var postDtos = _mapper.Map<List<PostSummaryDto>>(posts);
            return Result<List<PostSummaryDto>>.Success(postDtos);
        }
        catch (Exception ex)
        {
            return Result<List<PostSummaryDto>>.Failure($"Error getting trending posts: {ex.Message}");
        }
    }

    public async Task<Result<List<PostSummaryDto>>> GetUserFeedAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        try
        {
            var memberGroupIds = await _context.GroupMembers
                .Where(m => m.UserId == userId && m.IsActive)
                .Select(m => m.GroupId)
                .ToListAsync();

            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Group)
                .Include(p => p.Media)
                .Where(p => !p.IsDeleted && memberGroupIds.Contains(p.GroupId))
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var postDtos = _mapper.Map<List<PostSummaryDto>>(posts);
            return Result<List<PostSummaryDto>>.Success(postDtos);
        }
        catch (Exception ex)
        {
            return Result<List<PostSummaryDto>>.Failure($"Error getting user feed: {ex.Message}");
        }
    }
}