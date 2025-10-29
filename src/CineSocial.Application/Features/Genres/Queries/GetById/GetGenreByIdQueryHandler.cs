using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Genres.Queries.GetById;

public class GetGenreByIdQueryHandler : IRequestHandler<GetGenreByIdQuery, Result<Genre>>
{
    private readonly IApplicationDbContext _context;

    public GetGenreByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Genre>> Handle(GetGenreByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var genre = await _context.Genres
                .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

            if (genre == null)
                return Result<Genre>.Failure("Genre not found");

            return Result<Genre>.Success(genre);
        }
        catch (Exception ex)
        {
            return Result<Genre>.Failure($"Failed to retrieve genre: {ex.Message}");
        }
    }
}
