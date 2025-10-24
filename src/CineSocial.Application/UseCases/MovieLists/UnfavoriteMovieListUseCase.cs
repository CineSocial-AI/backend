using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.MovieLists;

public class UnfavoriteMovieListUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UnfavoriteMovieListUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> ExecuteAsync(int listId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        var favorite = await _context.MovieListFavorites
            .FirstOrDefaultAsync(mlf => mlf.UserId == currentUserId && mlf.MovieListId == listId, cancellationToken);

        if (favorite == null)
            throw new NotFoundException("Favorite not found");

        var movieList = await _context.MovieLists
            .FirstOrDefaultAsync(ml => ml.Id == listId && !ml.IsDeleted, cancellationToken);

        if (movieList != null)
        {
            movieList.FavoriteCount = Math.Max(0, movieList.FavoriteCount - 1);
        }

        _context.Remove(favorite);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
