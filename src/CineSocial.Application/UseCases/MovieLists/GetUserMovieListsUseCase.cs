using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.MovieLists;

public class GetUserMovieListsUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUserMovieListsUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<MovieList>> ExecuteAsync(int userId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId;
        var isOwnProfile = currentUserId == userId;

        var query = _context.MovieLists
            .Where(ml => ml.UserId == userId && !ml.IsDeleted);

        // If viewing someone else''s profile, only show public lists
        if (!isOwnProfile)
            query = query.Where(ml => ml.IsPublic);

        var lists = await query
            .OrderByDescending(ml => ml.CreatedAt)
            .ToListAsync(cancellationToken);

        return lists;
    }
}
