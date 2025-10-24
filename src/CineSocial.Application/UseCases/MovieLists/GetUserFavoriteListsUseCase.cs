using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.MovieLists;

public class GetUserFavoriteListsUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUserFavoriteListsUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<MovieList>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        var favoriteLists = await _context.MovieListFavorites
            .Where(mlf => mlf.UserId == currentUserId)
            .Join(_context.MovieLists,
                mlf => mlf.MovieListId,
                ml => ml.Id,
                (mlf, ml) => ml)
            .Where(ml => !ml.IsDeleted)
            .OrderByDescending(ml => ml.CreatedAt)
            .ToListAsync(cancellationToken);

        return favoriteLists;
    }
}
