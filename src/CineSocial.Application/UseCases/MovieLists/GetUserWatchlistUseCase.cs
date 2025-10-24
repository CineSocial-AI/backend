using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.MovieLists;

public class GetUserWatchlistUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUserWatchlistUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<MovieList> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        var watchlist = await _context.MovieLists
            .Include(ml => ml.Items)
            .ThenInclude(mli => mli.Movie)
            .FirstOrDefaultAsync(ml => ml.UserId == currentUserId && ml.IsWatchlist && !ml.IsDeleted, cancellationToken);

        if (watchlist == null)
        {
            // Create watchlist if not exists
            watchlist = new MovieList
            {
                UserId = currentUserId,
                Name = "Watchlist",
                Description = "My movies to watch",
                IsPublic = true,
                IsWatchlist = true
            };

            _context.Add(watchlist);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return watchlist;
    }
}
