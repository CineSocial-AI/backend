using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.MovieLists;

public class FavoriteMovieListUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public FavoriteMovieListUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
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

        if (!movieList.IsPublic && movieList.UserId != currentUserId)
            throw new ForbiddenException("Cannot favorite a private list");

        var existingFavorite = await _context.MovieListFavorites
            .FirstOrDefaultAsync(mlf => mlf.UserId == currentUserId && mlf.MovieListId == listId, cancellationToken);

        if (existingFavorite != null)
            throw new ConflictException("You have already favorited this list");

        var favorite = new MovieListFavorite
        {
            UserId = currentUserId,
            MovieListId = listId
        };

        _context.Add(favorite);

        // Update favorite count
        movieList.FavoriteCount++;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
