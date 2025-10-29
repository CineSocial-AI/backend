using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Lists.Commands.DeleteMovieList;

public class DeleteMovieListCommandHandler : IRequestHandler<DeleteMovieListCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteMovieListCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(DeleteMovieListCommand request, CancellationToken cancellationToken)
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

            if (movieList.UserId != currentUserId)
                return Result<bool>.Failure("You can only delete your own lists");

            if (movieList.IsWatchlist)
                return Result<bool>.Failure("Watchlist cannot be deleted");

            movieList.IsDeleted = true;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to delete movie list: {ex.Message}");
        }
    }
}
