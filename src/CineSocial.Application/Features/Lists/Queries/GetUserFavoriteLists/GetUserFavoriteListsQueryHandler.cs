using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Lists.Queries.GetUserFavoriteLists;

public class GetUserFavoriteListsQueryHandler : IRequestHandler<GetUserFavoriteListsQuery, Result<List<MovieList>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUserFavoriteListsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<MovieList>>> Handle(GetUserFavoriteListsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId == null)
                return Result<List<MovieList>>.Failure("User not authenticated");

            var favoriteLists = await _context.MovieListFavorites
                .Where(mlf => mlf.UserId == currentUserId)
                .Join(_context.MovieLists,
                    mlf => mlf.MovieListId,
                    ml => ml.Id,
                    (mlf, ml) => ml)
                .Where(ml => !ml.IsDeleted)
                .OrderByDescending(ml => ml.CreatedAt)
                .ToListAsync(cancellationToken);

            return Result<List<MovieList>>.Success(favoriteLists);
        }
        catch (Exception ex)
        {
            return Result<List<MovieList>>.Failure($"Failed to retrieve favorite lists: {ex.Message}");
        }
    }
}
