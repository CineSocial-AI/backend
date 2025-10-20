using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Blocks.Commands.Unblock;

public class UnblockCommandHandler : IRequestHandler<UnblockCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UnblockCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UnblockCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = 1; // TODO: Get from HttpContext

        var block = _context.Blocks
            .FirstOrDefault(b => b.BlockerId == currentUserId && b.BlockedUserId == request.BlockedUserId);

        if (block == null)
        {
            return Result.Failure("You have not blocked this user");
        }

        _context.Remove(block);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("User unblocked successfully");
    }
}
