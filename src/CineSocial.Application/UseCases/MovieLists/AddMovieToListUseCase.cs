using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.MovieLists;

public class AddMovieToListUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddMovieToListUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
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
            throw new ForbiddenException("You can only add movies to your own lists");

        // Check if movie already exists in list
        var existingItem = await _context.MovieListItems
            .FirstOrDefaultAsync(mli => mli.MovieListId == listId && mli.MovieId == movieId, cancellationToken);

        if (existingItem != null)
            throw new ConflictException("Movie is already in this list");

        // Get max order
        var maxOrder = await _context.MovieListItems
            .Where(mli => mli.MovieListId == listId)
            .MaxAsync(mli => (int?)mli.Order, cancellationToken) ?? 0;

        var movieListItem = new MovieListItem
        {
            MovieListId = listId,
            MovieId = movieId,
            Order = maxOrder + 1
        };

        _context.Add(movieListItem);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
