using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.People.Queries.GetFilmography;

public class GetPersonFilmographyQueryHandler : IRequestHandler<GetPersonFilmographyQuery, Result<object>>
{
    private readonly IApplicationDbContext _context;

    public GetPersonFilmographyQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<object>> Handle(GetPersonFilmographyQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var castRoles = await _context.MovieCasts
                .Where(mc => mc.PersonId == request.PersonId)
                .Include(mc => mc.Movie)
                .Select(mc => new
                {
                    movie = mc.Movie,
                    role = "Actor",
                    character = mc.Character,
                    order = mc.CastOrder
                })
                .ToListAsync(cancellationToken);

            var crewRoles = await _context.MovieCrews
                .Where(mc => mc.PersonId == request.PersonId)
                .Include(mc => mc.Movie)
                .Select(mc => new
                {
                    movie = mc.Movie,
                    role = mc.Job,
                    character = (string?)null,
                    order = (int?)null
                })
                .ToListAsync(cancellationToken);

            var filmography = castRoles.Concat(crewRoles)
                .OrderByDescending(f => f.movie.ReleaseDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var total = castRoles.Count + crewRoles.Count;

            var result = new
            {
                data = filmography,
                total,
                page = request.Page,
                pageSize = request.PageSize
            };

            return Result<object>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure($"Failed to retrieve filmography: {ex.Message}");
        }
    }
}
