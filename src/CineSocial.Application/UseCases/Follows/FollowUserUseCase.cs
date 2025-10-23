using CineSocial.Application.Common.Interfaces;

namespace CineSocial.Application.UseCases.Follows;

public class FollowUserUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public FollowUserUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> ExecuteAsync(int followingId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User not authenticated");

        if (currentUserId == followingId)
            throw new InvalidOperationException("You cannot follow yourself");

        var userToFollow = _context.Users
            .FirstOrDefault(u => u.Id == followingId && !u.IsDeleted);

        if (userToFollow == null)
            throw new InvalidOperationException("User not found");

        var existingFollow = _context.Follows
            .FirstOrDefault(f => f.FollowerId == currentUserId && f.FollowingId == followingId);

        if (existingFollow != null)
            throw new InvalidOperationException("You are already following this user");

        var isBlocked = _context.Blocks
            .Any(b => (b.BlockerId == currentUserId && b.BlockedUserId == followingId) ||
                      (b.BlockerId == followingId && b.BlockedUserId == currentUserId));

        if (isBlocked)
            throw new InvalidOperationException("Cannot follow this user");

        var follow = new Domain.Entities.User.Follow
        {
            FollowerId = currentUserId,
            FollowingId = followingId
        };

        _context.Add(follow);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
