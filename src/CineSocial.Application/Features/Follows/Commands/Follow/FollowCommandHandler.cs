using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.User;
using MediatR;

namespace CineSocial.Application.Features.Follows.Commands.Follow;

public class FollowCommandHandler : IRequestHandler<FollowCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public FollowCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(FollowCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = 1; // TODO: Get from HttpContext

        if (currentUserId == request.FollowingId)
        {
            return Result.Failure("You cannot follow yourself");
        }

        // Check if user to follow exists
        var userToFollow = _context.Users
            .FirstOrDefault(u => u.Id == request.FollowingId && !u.IsDeleted);

        if (userToFollow == null)
        {
            return Result.Failure("User not found");
        }

        // Check if already following
        var existingFollow = _context.Follows
            .FirstOrDefault(f => f.FollowerId == currentUserId && f.FollowingId == request.FollowingId);

        if (existingFollow != null)
        {
            return Result.Failure("You are already following this user");
        }

        // Check if blocked
        var isBlocked = _context.Blocks
            .Any(b => (b.BlockerId == currentUserId && b.BlockedUserId == request.FollowingId) ||
                          (b.BlockerId == request.FollowingId && b.BlockedUserId == currentUserId));

        if (isBlocked)
        {
            return Result.Failure("Cannot follow this user");
        }

        var follow = new Domain.Entities.User.Follow
        {
            FollowerId = currentUserId,
            FollowingId = request.FollowingId
        };

        _context.Add(follow);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("User followed successfully");
    }
}
