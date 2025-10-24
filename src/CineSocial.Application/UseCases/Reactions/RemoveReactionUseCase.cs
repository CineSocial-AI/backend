using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.Reactions;

public class RemoveReactionUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RemoveReactionUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> ExecuteAsync(int commentId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        var reaction = await _context.Reactions
            .FirstOrDefaultAsync(r => r.CommentId == commentId && r.UserId == currentUserId, cancellationToken);

        if (reaction == null)
        {
            throw new NotFoundException("Reaction not found");
        }

        _context.Remove(reaction);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
