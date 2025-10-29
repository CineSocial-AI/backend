using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Lists.Commands.ReorderMovieInList;

public class ReorderMovieInListCommandHandler : IRequestHandler<ReorderMovieInListCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ReorderMovieInListCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(ReorderMovieInListCommand request, CancellationToken cancellationToken)
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
                return Result<bool>.Failure("You can only reorder your own lists");

            var movieListItem = await _context.MovieListItems
                .FirstOrDefaultAsync(mli => mli.MovieListId == request.ListId && mli.MovieId == request.MovieId, cancellationToken);

            if (movieListItem == null)
                return Result<bool>.Failure("Movie not found in this list");

            if (request.NewOrder < 0)
                return Result<bool>.Failure("Order must be a positive number");

            movieListItem.Order = request.NewOrder;
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to reorder movie in list: {ex.Message}");
        }
    }
}
