using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Lists.Commands.AddMovieToList;

public class AddMovieToListCommandHandler : IRequestHandler<AddMovieToListCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddMovieToListCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(AddMovieToListCommand request, CancellationToken cancellationToken)
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
                return Result<bool>.Failure("You can only add movies to your own lists");

            // Check if movie already exists in list
            var existingItem = await _context.MovieListItems
                .FirstOrDefaultAsync(mli => mli.MovieListId == request.ListId && mli.MovieId == request.MovieId, cancellationToken);

            if (existingItem != null)
                return Result<bool>.Failure("Movie is already in this list");

            // Get max order
            var maxOrder = await _context.MovieListItems
                .Where(mli => mli.MovieListId == request.ListId)
                .MaxAsync(mli => (int?)mli.Order, cancellationToken) ?? 0;

            var movieListItem = new MovieListItem
            {
                MovieListId = request.ListId,
                MovieId = request.MovieId,
                Order = maxOrder + 1
            };

            _context.Add(movieListItem);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to add movie to list: {ex.Message}");
        }
    }
}
