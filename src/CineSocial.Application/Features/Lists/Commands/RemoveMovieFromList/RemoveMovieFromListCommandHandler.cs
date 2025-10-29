using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Lists.Commands.RemoveMovieFromList;

public class RemoveMovieFromListCommandHandler : IRequestHandler<RemoveMovieFromListCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RemoveMovieFromListCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(RemoveMovieFromListCommand request, CancellationToken cancellationToken)
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
                return Result<bool>.Failure("You can only remove movies from your own lists");

            var movieListItem = await _context.MovieListItems
                .FirstOrDefaultAsync(mli => mli.MovieListId == request.ListId && mli.MovieId == request.MovieId, cancellationToken);

            if (movieListItem == null)
                return Result<bool>.Failure("Movie not found in this list");

            _context.Remove(movieListItem);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to remove movie from list: {ex.Message}");
        }
    }
}
