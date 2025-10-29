using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Lists.Queries.GetUserWatchlist;

public class GetUserWatchlistQueryHandler : IRequestHandler<GetUserWatchlistQuery, Result<MovieList>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUserWatchlistQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<MovieList>> Handle(GetUserWatchlistQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId == null)
                return Result<MovieList>.Failure("User not authenticated");

            var watchlist = await _context.MovieLists
                .Include(ml => ml.Items)
                .ThenInclude(mli => mli.Movie)
                .FirstOrDefaultAsync(ml => ml.UserId == currentUserId && ml.IsWatchlist && !ml.IsDeleted, cancellationToken);

            if (watchlist == null)
            {
                // Create watchlist if not exists
                watchlist = new MovieList
                {
                    UserId = currentUserId.Value,
                    Name = "Watchlist",
                    Description = "My movies to watch",
                    IsPublic = true,
                    IsWatchlist = true
                };

                _context.Add(watchlist);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return Result<MovieList>.Success(watchlist);
        }
        catch (Exception ex)
        {
            return Result<MovieList>.Failure($"Failed to retrieve watchlist: {ex.Message}");
        }
    }
}
