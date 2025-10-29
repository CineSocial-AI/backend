using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Collections.Queries.GetMovies;

public class GetCollectionMoviesQueryHandler : IRequestHandler<GetCollectionMoviesQuery, Result<object>>
{
    private readonly IApplicationDbContext _context;

    public GetCollectionMoviesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<object>> Handle(GetCollectionMoviesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var movies = await _context.MovieCollections
                .Where(mc => mc.CollectionId == request.CollectionId)
                .Include(mc => mc.Movie)
                .Select(mc => mc.Movie)
                .OrderBy(m => m.ReleaseDate)
                .ToListAsync(cancellationToken);

            return Result<object>.Success(movies);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure($"Failed to retrieve collection movies: {ex.Message}");
        }
    }
}
