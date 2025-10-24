using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;

namespace CineSocial.Application.UseCases.Follows;

public class UnfollowUserUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UnfollowUserUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> ExecuteAsync(int followingId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        var follow = _context.Follows
            .FirstOrDefault(f => f.FollowerId == currentUserId && f.FollowingId == followingId);

        if (follow == null)
            throw new NotFoundException("Follow relationship not found");

        _context.Remove(follow);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
