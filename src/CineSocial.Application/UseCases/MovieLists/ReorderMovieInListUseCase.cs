using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.MovieLists;

public class ReorderMovieInListUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ReorderMovieInListUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> ExecuteAsync(int listId, int movieId, int newOrder, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        var movieList = await _context.MovieLists
            .FirstOrDefaultAsync(ml => ml.Id == listId && !ml.IsDeleted, cancellationToken);

        if (movieList == null)
            throw new NotFoundException("MovieList", listId);

        if (movieList.UserId != currentUserId)
            throw new ForbiddenException("You can only reorder your own lists");

        var movieListItem = await _context.MovieListItems
            .FirstOrDefaultAsync(mli => mli.MovieListId == listId && mli.MovieId == movieId, cancellationToken);

        if (movieListItem == null)
            throw new NotFoundException("Movie not found in this list");

        if (newOrder < 0)
            throw new ValidationException("newOrder", "Order must be a positive number");

        movieListItem.Order = newOrder;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
