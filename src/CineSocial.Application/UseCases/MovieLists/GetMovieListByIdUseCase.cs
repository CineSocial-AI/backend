using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.MovieLists;

public class GetMovieListByIdUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMovieListByIdUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<MovieList> ExecuteAsync(int listId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId;

        var movieList = await _context.MovieLists
            .Include(ml => ml.Items)
            .ThenInclude(mli => mli.Movie)
            .FirstOrDefaultAsync(ml => ml.Id == listId && !ml.IsDeleted, cancellationToken);

        if (movieList == null)
            throw new NotFoundException("MovieList", listId);

        // Private lists can only be viewed by owner
        if (!movieList.IsPublic && movieList.UserId != currentUserId)
            throw new ForbiddenException("This list is private");

        return movieList;
    }
}
