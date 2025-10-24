using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.MovieLists;

public class RemoveMovieFromListUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RemoveMovieFromListUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> ExecuteAsync(int listId, int movieId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        var movieList = await _context.MovieLists
            .FirstOrDefaultAsync(ml => ml.Id == listId && !ml.IsDeleted, cancellationToken);

        if (movieList == null)
            throw new NotFoundException("MovieList", listId);

        if (movieList.UserId != currentUserId)
            throw new ForbiddenException("You can only remove movies from your own lists");

        var movieListItem = await _context.MovieListItems
            .FirstOrDefaultAsync(mli => mli.MovieListId == listId && mli.MovieId == movieId, cancellationToken);

        if (movieListItem == null)
            throw new NotFoundException("Movie not found in this list");

        _context.Remove(movieListItem);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
