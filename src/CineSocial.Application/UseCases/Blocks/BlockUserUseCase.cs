using CineSocial.Application.Common.Interfaces;

namespace CineSocial.Application.UseCases.Blocks;

public class BlockUserUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public BlockUserUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> ExecuteAsync(int blockedUserId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User not authenticated");

        if (currentUserId == blockedUserId)
            throw new InvalidOperationException("You cannot block yourself");

        var userToBlock = _context.Users
            .FirstOrDefault(u => u.Id == blockedUserId && !u.IsDeleted);

        if (userToBlock == null)
            throw new InvalidOperationException("User not found");

        var existingBlock = _context.Blocks
            .FirstOrDefault(b => b.BlockerId == currentUserId && b.BlockedUserId == blockedUserId);

        if (existingBlock != null)
            throw new InvalidOperationException("You have already blocked this user");

        // Remove any existing follow relationships
        var followRelationships = _context.Follows
            .Where(f => (f.FollowerId == currentUserId && f.FollowingId == blockedUserId) ||
                       (f.FollowerId == blockedUserId && f.FollowingId == currentUserId))
            .ToList();

        if (followRelationships.Any())
        {
            _context.RemoveRange(followRelationships);
        }

        var block = new Domain.Entities.User.Block
        {
            BlockerId = currentUserId,
            BlockedUserId = blockedUserId
        };

        _context.Add(block);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
