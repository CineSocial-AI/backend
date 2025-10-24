using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.Comments;

public class ReplyToCommentUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ReplyToCommentUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Comment> ExecuteAsync(int parentCommentId, string content, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        if (string.IsNullOrWhiteSpace(content) || content.Length > 10000)
            throw new ValidationException("content", "Content must be between 1 and 10000 characters");

        var parentComment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == parentCommentId && !c.IsDeleted, cancellationToken);

        if (parentComment == null)
            throw new NotFoundException("Comment", parentCommentId);

        var reply = new Comment
        {
            UserId = currentUserId,
            Content = content.Trim(),
            CommentableType = parentComment.CommentableType,
            CommentableId = parentComment.CommentableId,
            ParentCommentId = parentCommentId,
            Depth = parentComment.Depth + 1
        };

        _context.Add(reply);
        await _context.SaveChangesAsync(cancellationToken);

        return reply;
    }
}
