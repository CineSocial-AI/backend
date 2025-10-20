using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Follows.Commands.Unfollow;

public class UnfollowCommandHandler : IRequestHandler<UnfollowCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UnfollowCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UnfollowCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = 1; // TODO: Get from HttpContext

        var follow = _context.Follows
            .FirstOrDefault(f => f.FollowerId == currentUserId && f.FollowingId == request.FollowingId);

        if (follow == null)
        {
            return Result.Failure("You are not following this user");
        }

        _context.Remove(follow);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("User unfollowed successfully");
    }
}
