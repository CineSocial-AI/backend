using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CineSocial.Application.Features.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateCommentCommandHandler> _logger;

    public CreateCommentCommandHandler(
        IApplicationDbContext context,
        ILogger<CreateCommentCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = 1; // TODO: Get from ICurrentUserService

        _logger.LogInformation("Creating comment: UserId={UserId}, CommentableType={CommentableType}, CommentableId={CommentableId}, ContentLength={ContentLength}",
            currentUserId, request.CommentableType, request.CommentableId, request.Content?.Length ?? 0);

        if (string.IsNullOrWhiteSpace(request.Content) || request.Content.Length > 10000)
        {
            _logger.LogWarning("Invalid comment content: UserId={UserId}, ContentLength={ContentLength}",
                currentUserId, request.Content?.Length ?? 0);
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

        _logger.LogInformation("Comment created successfully: CommentId={CommentId}, UserId={UserId}, CommentableType={CommentableType}, CommentableId={CommentableId}",
            comment.Id, currentUserId, request.CommentableType, request.CommentableId);

        return Result<int>.Success(comment.Id);
    }
}
