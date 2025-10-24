using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.MovieLists;

public class GetPublicMovieListsUseCase
{
    private readonly IApplicationDbContext _context;

    public GetPublicMovieListsUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MovieList>> ExecuteAsync(
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        var lists = await _context.MovieLists
            .Where(ml => ml.IsPublic && !ml.IsDeleted && !ml.IsWatchlist)
            .OrderByDescending(ml => ml.FavoriteCount)
            .ThenByDescending(ml => ml.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return lists;
    }
}
