using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Lists.Queries.GetPublicMovieLists;

public class GetPublicMovieListsQueryHandler : IRequestHandler<GetPublicMovieListsQuery, Result<List<MovieList>>>
{
    private readonly IApplicationDbContext _context;

    public GetPublicMovieListsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<MovieList>>> Handle(GetPublicMovieListsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var skip = (request.Page - 1) * request.PageSize;

            var lists = await _context.MovieLists
                .Where(ml => ml.IsPublic && !ml.IsDeleted && !ml.IsWatchlist)
                .OrderByDescending(ml => ml.FavoriteCount)
                .ThenByDescending(ml => ml.CreatedAt)
                .Skip(skip)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return Result<List<MovieList>>.Success(lists);
        }
        catch (Exception ex)
        {
            return Result<List<MovieList>>.Failure($"Failed to retrieve public movie lists: {ex.Message}");
        }
    }
}
