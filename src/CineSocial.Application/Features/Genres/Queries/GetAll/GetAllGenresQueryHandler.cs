using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Genres.Queries.GetAll;

public class GetAllGenresQueryHandler : IRequestHandler<GetAllGenresQuery, Result<List<Genre>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllGenresQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<Genre>>> Handle(GetAllGenresQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var genres = await _context.Genres
                .OrderBy(g => g.Name)
                .ToListAsync(cancellationToken);

            return Result<List<Genre>>.Success(genres);
        }
        catch (Exception ex)
        {
            return Result<List<Genre>>.Failure($"Failed to retrieve genres: {ex.Message}");
        }
    }
}
