using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.MovieLists;

public class DeleteMovieListUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteMovieListUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> ExecuteAsync(int listId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        var movieList = await _context.MovieLists
            .FirstOrDefaultAsync(ml => ml.Id == listId && !ml.IsDeleted, cancellationToken);

        if (movieList == null)
            throw new NotFoundException("MovieList", listId);

        if (movieList.UserId != currentUserId)
            throw new ForbiddenException("You can only delete your own lists");

        if (movieList.IsWatchlist)
            throw new BusinessException("Watchlist cannot be deleted", "BUSINESS_001");

        movieList.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
