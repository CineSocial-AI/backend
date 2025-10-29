using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Lists.Commands.UpdateMovieList;

public class UpdateMovieListCommandHandler : IRequestHandler<UpdateMovieListCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateMovieListCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(UpdateMovieListCommand request, CancellationToken cancellationToken)
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
                return Result<bool>.Failure("You can only update your own lists");

            if (request.Name != null)
            {
                if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 200)
                    return Result<bool>.Failure("Name must be between 1 and 200 characters");
                movieList.Name = request.Name.Trim();
            }

            if (request.Description != null)
            {
                if (request.Description.Length > 1000)
                    return Result<bool>.Failure("Description must be less than 1000 characters");
                movieList.Description = request.Description.Trim();
            }

            if (request.IsPublic.HasValue)
                movieList.IsPublic = request.IsPublic.Value;

            if (request.CoverImageId.HasValue)
                movieList.CoverImageId = request.CoverImageId.Value;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to update movie list: {ex.Message}");
        }
    }
}
