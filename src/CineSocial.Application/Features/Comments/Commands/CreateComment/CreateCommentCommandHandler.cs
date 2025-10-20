using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public CreateCommentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = 1;

        if (string.IsNullOrWhiteSpace(request.Content) || request.Content.Length > 10000)
        {
            return Result<int>.Failure("Content must be between 1 and 10000 characters");
        }

        var comment = new Comment
        {
            UserId = currentUserId,
            Content = request.Content.Trim(),
            CommentableType = request.CommentableType,
            CommentableId = request.CommentableId,
            Depth = 0
        };

        _context.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(comment.Id);
    }
}
