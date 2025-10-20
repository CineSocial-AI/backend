using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Reactions.Commands.RemoveReaction;

public class RemoveReactionCommandHandler : IRequestHandler<RemoveReactionCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public RemoveReactionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(RemoveReactionCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = 1;

        var reaction = await _context.Reactions
            .FirstOrDefaultAsync(r => r.CommentId == request.CommentId && r.UserId == currentUserId, cancellationToken);

        if (reaction == null)
        {
            return Result.Failure("Reaction not found");
        }

        _context.Remove(reaction);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Reaction removed successfully");
    }
}
