using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Genres.Queries.GetMovies;

public class GetGenreMoviesQueryHandler : IRequestHandler<GetGenreMoviesQuery, Result<object>>
{
    private readonly IApplicationDbContext _context;

    public GetGenreMoviesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<object>> Handle(GetGenreMoviesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.MovieGenres
                .Where(mg => mg.GenreId == request.GenreId);

            var total = await query.CountAsync(cancellationToken);

            var movies = await query
                .Include(mg => mg.Movie)
                .Select(mg => mg.Movie)
                .OrderByDescending(m => m.Popularity)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var result = new
            {
                data = movies,
                total,
                page = request.Page,
                pageSize = request.PageSize
            };

            return Result<object>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure($"Failed to retrieve genre movies: {ex.Message}");
        }
    }
}
