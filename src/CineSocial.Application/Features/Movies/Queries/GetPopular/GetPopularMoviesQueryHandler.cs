using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Movies.Queries.GetPopular;

public class GetPopularMoviesQueryHandler : IRequestHandler<GetPopularMoviesQuery, Result<object>>
{
    private readonly IRepository<MovieEntity> _movieRepository;

    public GetPopularMoviesQueryHandler(IRepository<MovieEntity> movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<Result<object>> Handle(GetPopularMoviesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _movieRepository.GetQueryable();
            var total = await query.CountAsync(cancellationToken);

            var movies = await query
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
            return Result<object>.Failure($"Failed to retrieve popular movies: {ex.Message}");
        }
    }
}
