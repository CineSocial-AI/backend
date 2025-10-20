using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Comments.Commands.ReplyToComment;

public class ReplyToCommentCommandHandler : IRequestHandler<ReplyToCommentCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public ReplyToCommentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(ReplyToCommentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = 1;

        if (string.IsNullOrWhiteSpace(request.Content) || request.Content.Length > 10000)
        {
            return Result<int>.Failure("Content must be between 1 and 10000 characters");
        }

        var parentComment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == request.ParentCommentId && !c.IsDeleted, cancellationToken);

        if (parentComment == null)
        {
            return Result<int>.Failure("Parent comment not found");
        }

        var reply = new Comment
        {
            UserId = currentUserId,
            Content = request.Content.Trim(),
            CommentableType = parentComment.CommentableType,
            CommentableId = parentComment.CommentableId,
            ParentCommentId = request.ParentCommentId,
            Depth = parentComment.Depth + 1
        };

        _context.Add(reply);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(reply.Id);
    }
}
