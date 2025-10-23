using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using CineSocial.Domain.Enums;

namespace CineSocial.Application.UseCases.Comments;

public class CreateCommentUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateCommentUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Comment> ExecuteAsync(
        CommentableType commentableType,
        int commentableId,
        string content,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User not authenticated");

        if (string.IsNullOrWhiteSpace(content) || content.Length > 10000)
            throw new ArgumentException("Content must be between 1 and 10000 characters");

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

        return comment;
    }
}
