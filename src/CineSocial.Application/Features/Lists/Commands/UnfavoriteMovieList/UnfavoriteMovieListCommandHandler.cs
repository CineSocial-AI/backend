using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Lists.Commands.UnfavoriteMovieList;

public class UnfavoriteMovieListCommandHandler : IRequestHandler<UnfavoriteMovieListCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UnfavoriteMovieListCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(UnfavoriteMovieListCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId == null)
                return Result<bool>.Failure("User not authenticated");

            var favorite = await _context.MovieListFavorites
                .FirstOrDefaultAsync(mlf => mlf.UserId == currentUserId && mlf.MovieListId == request.ListId, cancellationToken);

            if (favorite == null)
                return Result<bool>.Failure("Favorite not found");

            var movieList = await _context.MovieLists
                .FirstOrDefaultAsync(ml => ml.Id == request.ListId && !ml.IsDeleted, cancellationToken);

            if (movieList != null)
            {
                movieList.FavoriteCount = Math.Max(0, movieList.FavoriteCount - 1);
            }

            _context.Remove(favorite);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to unfavorite movie list: {ex.Message}");
        }
    }
}
