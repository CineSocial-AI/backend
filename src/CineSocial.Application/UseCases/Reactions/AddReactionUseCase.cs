using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using CineSocial.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.Reactions;

public class AddReactionUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddReactionUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> ExecuteAsync(int commentId, ReactionType type, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User not authenticated");

        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted, cancellationToken);

        if (comment == null)
        {
            throw new InvalidOperationException("Comment not found");
        }

        var existingReaction = await _context.Reactions
            .FirstOrDefaultAsync(r => r.CommentId == commentId && r.UserId == currentUserId, cancellationToken);

        if (existingReaction != null)
        {
            if (existingReaction.Type == type)
            {
                throw new InvalidOperationException("You have already reacted with this type");
            }

            existingReaction.Type = type;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        var reaction = new Reaction
        {
            UserId = currentUserId,
            CommentId = commentId,
            Type = type
        };

        _context.Add(reaction);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
