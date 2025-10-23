using CineSocial.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.Rates;

public class RemoveRateUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RemoveRateUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> ExecuteAsync(int movieId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User not authenticated");

        var rate = await _context.Rates
            .FirstOrDefaultAsync(r => r.MovieId == movieId && r.UserId == currentUserId, cancellationToken);

        if (rate == null)
        {
            throw new InvalidOperationException("Rating not found");
        }

        _context.Remove(rate);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
