using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.Comments;

public class UpdateCommentUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCommentUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> ExecuteAsync(int commentId, string content, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        if (string.IsNullOrWhiteSpace(content) || content.Length > 10000)
        {
            throw new ValidationException("content", "Content must be between 1 and 10000 characters");
        }

        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted, cancellationToken);

        if (comment == null)
        {
            throw new NotFoundException("Comment", commentId);
        }

        if (comment.UserId != currentUserId)
        {
            throw new ForbiddenException("You can only edit your own comments");
        }

        comment.Content = content.Trim();
        comment.IsEdited = true;
        comment.EditedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
