using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;

namespace CineSocial.Application.UseCases.Blocks;

public class UnblockUserUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UnblockUserUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> ExecuteAsync(int blockedUserId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        var block = _context.Blocks
            .FirstOrDefault(b => b.BlockerId == currentUserId && b.BlockedUserId == blockedUserId);

        if (block == null)
            throw new NotFoundException("Block relationship not found");

        _context.Remove(block);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
