using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CineSocial.Application.UseCases.Blocks;

public class BlockUserUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<BlockUserUseCase> _logger;

    public BlockUserUseCase(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<BlockUserUseCase> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> ExecuteAsync(int blockedUserId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        _logger.LogInformation("User block attempt: BlockerId={BlockerId}, BlockedUserId={BlockedUserId}",
            currentUserId, blockedUserId);

        if (currentUserId == blockedUserId)
        {
            _logger.LogWarning("User tried to block themselves: UserId={UserId}", currentUserId);
            throw new BusinessException("You cannot block yourself", "BUSINESS_004");
        }

        var userToBlock = _context.Users
            .FirstOrDefault(u => u.Id == blockedUserId && !u.IsDeleted);

        if (userToBlock == null)
            throw new NotFoundException("User", blockedUserId);

        var existingBlock = _context.Blocks
            .FirstOrDefault(b => b.BlockerId == currentUserId && b.BlockedUserId == blockedUserId);

        if (existingBlock != null)
            throw new ConflictException("You have already blocked this user");

        // Remove any existing follow relationships
        var followRelationships = _context.Follows
            .Where(f => (f.FollowerId == currentUserId && f.FollowingId == blockedUserId) ||
                       (f.FollowerId == blockedUserId && f.FollowingId == currentUserId))
            .ToList();

        if (followRelationships.Any())
        {
            _logger.LogInformation("Removing {FollowCount} follow relationships during block: BlockerId={BlockerId}, BlockedUserId={BlockedUserId}",
                followRelationships.Count, currentUserId, blockedUserId);
            _context.RemoveRange(followRelationships);
        }

        var block = new Domain.Entities.User.Block
        {
            BlockerId = currentUserId,
            BlockedUserId = blockedUserId
        };

        _context.Add(block);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User blocked successfully: BlockerId={BlockerId}, BlockedUserId={BlockedUserId}",
            currentUserId, blockedUserId);

        return true;
    }
}
