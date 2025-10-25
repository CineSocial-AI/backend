using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CineSocial.Application.UseCases.Follows;

public class FollowUserUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<FollowUserUseCase> _logger;

    public FollowUserUseCase(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<FollowUserUseCase> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> ExecuteAsync(int followingId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        _logger.LogInformation("User follow attempt: FollowerId={FollowerId}, FollowingId={FollowingId}",
            currentUserId, followingId);

        if (currentUserId == followingId)
        {
            _logger.LogWarning("User tried to follow themselves: UserId={UserId}", currentUserId);
            throw new BusinessException("You cannot follow yourself", "BUSINESS_004");
        }

        var userToFollow = _context.Users
            .FirstOrDefault(u => u.Id == followingId && !u.IsDeleted);

        if (userToFollow == null)
            throw new NotFoundException("User", followingId);

        var existingFollow = _context.Follows
            .FirstOrDefault(f => f.FollowerId == currentUserId && f.FollowingId == followingId);

        if (existingFollow != null)
            throw new ConflictException("You are already following this user");

        var isBlocked = _context.Blocks
            .Any(b => (b.BlockerId == currentUserId && b.BlockedUserId == followingId) ||
                      (b.BlockerId == followingId && b.BlockedUserId == currentUserId));

        if (isBlocked)
            throw new BusinessException("Cannot follow this user", "BUSINESS_003");

        var follow = new Domain.Entities.User.Follow
        {
            FollowerId = currentUserId,
            FollowingId = followingId
        };

        _context.Add(follow);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User followed successfully: FollowerId={FollowerId}, FollowingId={FollowingId}",
            currentUserId, followingId);

        return true;
    }
}
