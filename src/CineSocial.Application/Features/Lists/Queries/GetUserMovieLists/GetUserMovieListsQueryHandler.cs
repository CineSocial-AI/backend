using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Lists.Queries.GetUserMovieLists;

public class GetUserMovieListsQueryHandler : IRequestHandler<GetUserMovieListsQuery, Result<List<MovieList>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUserMovieListsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<MovieList>>> Handle(GetUserMovieListsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _currentUserService.UserId;
            var isOwnProfile = currentUserId == request.UserId;

            var query = _context.MovieLists
                .Where(ml => ml.UserId == request.UserId && !ml.IsDeleted);

            // If viewing someone else's profile, only show public lists
            if (!isOwnProfile)
                query = query.Where(ml => ml.IsPublic);

            var lists = await query
                .OrderByDescending(ml => ml.CreatedAt)
                .ToListAsync(cancellationToken);

            return Result<List<MovieList>>.Success(lists);
        }
        catch (Exception ex)
        {
            return Result<List<MovieList>>.Failure($"Failed to retrieve user movie lists: {ex.Message}");
        }
    }
}
