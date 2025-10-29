using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Movies.Queries.GetTopRated;

public class GetTopRatedMoviesQueryHandler : IRequestHandler<GetTopRatedMoviesQuery, Result<object>>
{
    private readonly IRepository<MovieEntity> _movieRepository;

    public GetTopRatedMoviesQueryHandler(IRepository<MovieEntity> movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<Result<object>> Handle(GetTopRatedMoviesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _movieRepository.GetQueryable()
                .Where(m => m.VoteCount >= request.MinVotes);

            var total = await query.CountAsync(cancellationToken);

            var movies = await query
                .OrderByDescending(m => m.VoteAverage)
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
            return Result<object>.Failure($"Failed to retrieve top rated movies: {ex.Message}");
        }
    }
}
