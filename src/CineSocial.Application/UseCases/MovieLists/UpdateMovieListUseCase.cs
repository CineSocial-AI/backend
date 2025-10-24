using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.MovieLists;

public class UpdateMovieListUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateMovieListUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> ExecuteAsync(
        int listId,
        string? name,
        string? description,
        bool? isPublic,
        int? coverImageId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        var movieList = await _context.MovieLists
            .FirstOrDefaultAsync(ml => ml.Id == listId && !ml.IsDeleted, cancellationToken);

        if (movieList == null)
            throw new NotFoundException("MovieList", listId);

        if (movieList.UserId != currentUserId)
            throw new ForbiddenException("You can only update your own lists");

        if (name != null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 200)
                throw new ValidationException("name", "Name must be between 1 and 200 characters");
            movieList.Name = name.Trim();
        }

        if (description != null)
        {
            if (description.Length > 1000)
                throw new ValidationException("description", "Description must be less than 1000 characters");
            movieList.Description = description.Trim();
        }

        if (isPublic.HasValue)
            movieList.IsPublic = isPublic.Value;

        if (coverImageId.HasValue)
            movieList.CoverImageId = coverImageId.Value;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
