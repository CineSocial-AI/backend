using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Comments.Commands.UpdateComment;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateCommentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = 1;

        if (string.IsNullOrWhiteSpace(request.Content) || request.Content.Length > 10000)
        {
            return Result.Failure("Content must be between 1 and 10000 characters");
        }

        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == request.CommentId && !c.IsDeleted, cancellationToken);

        if (comment == null)
        {
            return Result.Failure("Comment not found");
        }

        if (comment.UserId != currentUserId)
        {
            return Result.Failure("You can only edit your own comments");
        }

        comment.Content = request.Content.Trim();
        comment.IsEdited = true;
        comment.EditedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Comment updated successfully");
    }
}
