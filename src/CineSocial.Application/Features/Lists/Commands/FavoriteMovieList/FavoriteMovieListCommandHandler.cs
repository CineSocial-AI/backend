using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Lists.Commands.FavoriteMovieList;

public class FavoriteMovieListCommandHandler : IRequestHandler<FavoriteMovieListCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public FavoriteMovieListCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(FavoriteMovieListCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId == null)
                return Result<bool>.Failure("User not authenticated");

            var movieList = await _context.MovieLists
                .FirstOrDefaultAsync(ml => ml.Id == request.ListId && !ml.IsDeleted, cancellationToken);

            if (movieList == null)
                return Result<bool>.Failure("List not found");

            if (!movieList.IsPublic && movieList.UserId != currentUserId)
                return Result<bool>.Failure("Cannot favorite a private list");

            var existingFavorite = await _context.MovieListFavorites
                .FirstOrDefaultAsync(mlf => mlf.UserId == currentUserId && mlf.MovieListId == request.ListId, cancellationToken);

            if (existingFavorite != null)
                return Result<bool>.Failure("You have already favorited this list");

            var favorite = new MovieListFavorite
            {
                UserId = currentUserId.Value,
                MovieListId = request.ListId
            };

            _context.Add(favorite);

            // Update favorite count
            movieList.FavoriteCount++;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to favorite movie list: {ex.Message}");
        }
    }
}
