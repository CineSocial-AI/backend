using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Reactions.Commands.AddReaction;

public class AddReactionCommandHandler : IRequestHandler<AddReactionCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public AddReactionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(AddReactionCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = 1;

        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == request.CommentId && !c.IsDeleted, cancellationToken);

        if (comment == null)
        {
            return Result.Failure("Comment not found");
        }

        var existingReaction = await _context.Reactions
            .FirstOrDefaultAsync(r => r.CommentId == request.CommentId && r.UserId == currentUserId, cancellationToken);

        if (existingReaction != null)
        {
            if (existingReaction.Type == request.Type)
            {
                return Result.Failure("You have already reacted with this type");
            }

            existingReaction.Type = request.Type;
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success("Reaction updated successfully");
        }

        var reaction = new Reaction
        {
            UserId = currentUserId,
            CommentId = request.CommentId,
            Type = request.Type
        };

        _context.Add(reaction);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Reaction added successfully");
    }
}
