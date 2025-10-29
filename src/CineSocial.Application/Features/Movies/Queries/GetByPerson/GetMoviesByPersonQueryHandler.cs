using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Movies.Queries.GetByPerson;

public class GetMoviesByPersonQueryHandler : IRequestHandler<GetMoviesByPersonQuery, Result<object>>
{
    private readonly IApplicationDbContext _context;
    private readonly IRepository<MovieEntity> _movieRepository;

    public GetMoviesByPersonQueryHandler(
        IApplicationDbContext context,
        IRepository<MovieEntity> movieRepository)
    {
        _context = context;
        _movieRepository = movieRepository;
    }

    public async Task<Result<object>> Handle(GetMoviesByPersonQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var castMovieIds = await _context.MovieCasts
                .Where(mc => mc.PersonId == request.PersonId)
                .Select(mc => mc.MovieId)
                .ToListAsync(cancellationToken);

            var crewMovieIds = await _context.MovieCrews
                .Where(mc => mc.PersonId == request.PersonId)
                .Select(mc => mc.MovieId)
                .ToListAsync(cancellationToken);

            var allMovieIds = castMovieIds.Concat(crewMovieIds).Distinct().ToList();
            var total = allMovieIds.Count;

            var movies = await _movieRepository.GetQueryable()
                .Where(m => allMovieIds.Contains(m.Id))
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
            return Result<object>.Failure($"Failed to retrieve movies by person: {ex.Message}");
        }
    }
}
