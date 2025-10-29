using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Movies.Queries.GetNewReleases;

public class GetNewReleasesQueryHandler : IRequestHandler<GetNewReleasesQuery, Result<object>>
{
    private readonly IRepository<MovieEntity> _movieRepository;

    public GetNewReleasesQueryHandler(IRepository<MovieEntity> movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<Result<object>> Handle(GetNewReleasesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-request.Days);

            var query = _movieRepository.GetQueryable()
                .Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value >= cutoffDate);

            var total = await query.CountAsync(cancellationToken);

            var movies = await query
                .OrderByDescending(m => m.ReleaseDate)
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
            return Result<object>.Failure($"Failed to retrieve new releases: {ex.Message}");
        }
    }
}
