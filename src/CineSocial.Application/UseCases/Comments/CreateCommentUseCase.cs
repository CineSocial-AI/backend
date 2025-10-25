using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using CineSocial.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CineSocial.Application.UseCases.Comments;

public class CreateCommentUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateCommentUseCase> _logger;

    public CreateCommentUseCase(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<CreateCommentUseCase> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Comment> ExecuteAsync(
        CommentableType commentableType,
        int commentableId,
        string content,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        _logger.LogInformation("Creating comment: UserId={UserId}, CommentableType={CommentableType}, CommentableId={CommentableId}, ContentLength={ContentLength}",
            currentUserId, commentableType, commentableId, content?.Length ?? 0);

        if (string.IsNullOrWhiteSpace(content) || content.Length > 10000)
        {
            _logger.LogWarning("Invalid comment content: UserId={UserId}, ContentLength={ContentLength}",
                currentUserId, content?.Length ?? 0);
            throw new ValidationException("content", "Content must be between 1 and 10000 characters");
        }

        var comment = new Comment
        {
            UserId = currentUserId,
            Content = content.Trim(),
            CommentableType = commentableType,
            CommentableId = commentableId,
            Depth = 0
        };

        _context.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Comment created successfully: CommentId={CommentId}, UserId={UserId}, CommentableType={CommentableType}, CommentableId={CommentableId}",
            comment.Id, currentUserId, commentableType, commentableId);

        return comment;
    }
}
