using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CineSocial.Core.Application.Ports;
using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.DTOs.Reviews;
using CineSocial.Core.Domain.Entities;
using CineSocial.Adapters.Infrastructure.Database;

namespace CineSocial.Adapters.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ReviewService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<ReviewDto>>> GetReviewsAsync(int page = 1, int pageSize = 20, Guid? movieId = null, Guid? userId = null)
    {
        try
        {
            var query = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Movie)
                .Include(r => r.Comments)
                .AsQueryable();

            if (movieId.HasValue)
                query = query.Where(r => r.MovieId == movieId.Value);
            if (userId.HasValue)
                query = query.Where(r => r.UserId == userId.Value);

            query = query.OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync();
            var reviews = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var reviewDtos = _mapper.Map<List<ReviewDto>>(reviews);
            var result = new PagedResult<ReviewDto>(reviewDtos, totalCount, page, pageSize);
            return Result<PagedResult<ReviewDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<ReviewDto>>.Failure($"Error getting reviews: {ex.Message}");
        }
    }

    public async Task<Result<ReviewDto>> GetReviewByIdAsync(Guid id, Guid? currentUserId = null)
    {
        try
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Movie)
                .Include(r => r.Comments)
                .Include(r => r.ReviewLikes)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return Result<ReviewDto>.Failure("Review not found");

            var reviewDto = _mapper.Map<ReviewDto>(review);
            return Result<ReviewDto>.Success(reviewDto);
        }
        catch (Exception ex)
        {
            return Result<ReviewDto>.Failure($"Error getting review: {ex.Message}");
        }
    }

    public async Task<Result<ReviewDto>> CreateReviewAsync(Guid userId, CreateReviewDto createDto)
    {
        try
        {
            var review = _mapper.Map<Review>(createDto);
            review.Id = Guid.NewGuid();
            review.UserId = userId;
            review.CreatedAt = DateTime.UtcNow;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return await GetReviewByIdAsync(review.Id, userId);
        }
        catch (Exception ex)
        {
            return Result<ReviewDto>.Failure($"Error creating review: {ex.Message}");
        }
    }

    public async Task<Result<ReviewDto>> UpdateReviewAsync(Guid userId, Guid reviewId, UpdateReviewDto updateDto)
    {
        try
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);

            if (review == null)
                return Result<ReviewDto>.Failure("Review not found");

            _mapper.Map(updateDto, review);
            review.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetReviewByIdAsync(review.Id, userId);
        }
        catch (Exception ex)
        {
            return Result<ReviewDto>.Failure($"Error updating review: {ex.Message}");
        }
    }

    public async Task<Result> DeleteReviewAsync(Guid userId, Guid reviewId)
    {
        try
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);

            if (review == null)
                return Result.Failure("Review not found");

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting review: {ex.Message}");
        }
    }

    public async Task<Result> LikeReviewAsync(Guid userId, Guid reviewId, bool isLike)
    {
        try
        {
            var existingLike = await _context.ReviewLikes
                .FirstOrDefaultAsync(rl => rl.UserId == userId && rl.ReviewId == reviewId);

            if (existingLike != null)
            {
                existingLike.IsLike = isLike;
            }
            else
            {
                var reviewLike = new ReviewLike
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ReviewId = reviewId,
                    IsLike = isLike,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ReviewLikes.Add(reviewLike);
            }

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error liking review: {ex.Message}");
        }
    }

    public async Task<Result> RemoveLikeAsync(Guid userId, Guid reviewId)
    {
        try
        {
            var existingLike = await _context.ReviewLikes
                .FirstOrDefaultAsync(rl => rl.UserId == userId && rl.ReviewId == reviewId);

            if (existingLike != null)
            {
                _context.ReviewLikes.Remove(existingLike);
                await _context.SaveChangesAsync();
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error removing like: {ex.Message}");
        }
    }

    public async Task<Result<PagedResult<CommentDto>>> GetCommentsAsync(Guid reviewId, int page = 1, int pageSize = 20, Guid? currentUserId = null)
    {
        try
        {
            var query = _context.Comments
                .Include(c => c.User)
                .Include(c => c.Replies)
                .ThenInclude(r => r.User)
                .Where(c => c.ReviewId == reviewId && c.ParentCommentId == null)
                .OrderBy(c => c.CreatedAt);

            var totalCount = await query.CountAsync();
            var comments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var commentDtos = _mapper.Map<List<CommentDto>>(comments);
            var result = new PagedResult<CommentDto>(commentDtos, totalCount, page, pageSize);
            return Result<PagedResult<CommentDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<CommentDto>>.Failure($"Error getting comments: {ex.Message}");
        }
    }

    public async Task<Result<CommentDto>> CreateCommentAsync(Guid userId, CreateCommentDto createDto)
    {
        try
        {
            var comment = _mapper.Map<Comment>(createDto);
            comment.Id = Guid.NewGuid();
            comment.UserId = userId;
            comment.CreatedAt = DateTime.UtcNow;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var createdComment = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Replies)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            var commentDto = _mapper.Map<CommentDto>(createdComment);
            return Result<CommentDto>.Success(commentDto);
        }
        catch (Exception ex)
        {
            return Result<CommentDto>.Failure($"Error creating comment: {ex.Message}");
        }
    }

    public async Task<Result<CommentDto>> UpdateCommentAsync(Guid userId, Guid commentId, UpdateCommentDto updateDto)
    {
        try
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);

            if (comment == null)
                return Result<CommentDto>.Failure("Comment not found");

            _mapper.Map(updateDto, comment);
            comment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var updatedComment = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Replies)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            var commentDto = _mapper.Map<CommentDto>(updatedComment);
            return Result<CommentDto>.Success(commentDto);
        }
        catch (Exception ex)
        {
            return Result<CommentDto>.Failure($"Error updating comment: {ex.Message}");
        }
    }

    public async Task<Result> DeleteCommentAsync(Guid userId, Guid commentId)
    {
        try
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);

            if (comment == null)
                return Result.Failure("Comment not found");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting comment: {ex.Message}");
        }
    }

    public async Task<Result> LikeCommentAsync(Guid userId, Guid commentId, bool isLike)
    {
        try
        {
            var existingLike = await _context.CommentLikes
                .FirstOrDefaultAsync(cl => cl.UserId == userId && cl.CommentId == commentId);

            if (existingLike != null)
            {
                existingLike.IsLike = isLike;
            }
            else
            {
                var commentLike = new CommentLike
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CommentId = commentId,
                    IsLike = isLike,
                    CreatedAt = DateTime.UtcNow
                };
                _context.CommentLikes.Add(commentLike);
            }

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error liking comment: {ex.Message}");
        }
    }

    public async Task<Result> RemoveCommentLikeAsync(Guid userId, Guid commentId)
    {
        try
        {
            var existingLike = await _context.CommentLikes
                .FirstOrDefaultAsync(cl => cl.UserId == userId && cl.CommentId == commentId);

            if (existingLike != null)
            {
                _context.CommentLikes.Remove(existingLike);
                await _context.SaveChangesAsync();
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error removing comment like: {ex.Message}");
        }
    }
}

