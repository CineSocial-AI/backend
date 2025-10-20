using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Blocks.Commands.Block;

public class BlockCommandHandler : IRequestHandler<BlockCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public BlockCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(BlockCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = 1; // TODO: Get from HttpContext

        if (currentUserId == request.BlockedUserId)
        {
            return Result.Failure("You cannot block yourself");
        }

        // Check if user to block exists
        var userToBlock = _context.Users
            .FirstOrDefault(u => u.Id == request.BlockedUserId && !u.IsDeleted);

        if (userToBlock == null)
        {
            return Result.Failure("User not found");
        }

        // Check if already blocked
        var existingBlock = _context.Blocks
            .FirstOrDefault(b => b.BlockerId == currentUserId && b.BlockedUserId == request.BlockedUserId);

        if (existingBlock != null)
        {
            return Result.Failure("You have already blocked this user");
        }

        // Remove any existing follow relationships
        var followRelationships = _context.Follows
            .Where(f => (f.FollowerId == currentUserId && f.FollowingId == request.BlockedUserId) ||
                       (f.FollowerId == request.BlockedUserId && f.FollowingId == currentUserId))
            .ToList();

        if (followRelationships.Any())
        {
            _context.RemoveRange(followRelationships);
        }

        var block = new Domain.Entities.User.Block
        {
            BlockerId = currentUserId,
            BlockedUserId = request.BlockedUserId
        };

        _context.Add(block);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("User blocked successfully");
    }
}
