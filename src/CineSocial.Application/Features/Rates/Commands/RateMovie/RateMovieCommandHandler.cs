using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Rates.Commands.RateMovie;

public class RateMovieCommandHandler : IRequestHandler<RateMovieCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public RateMovieCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(RateMovieCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = 1;

        if (request.Rating < 0 || request.Rating > 10)
        {
            return Result.Failure("Rating must be between 0 and 10");
        }

        var existingRate = await _context.Rates
            .FirstOrDefaultAsync(r => r.MovieId == request.MovieId && r.UserId == currentUserId, cancellationToken);

        if (existingRate != null)
        {
            existingRate.Rating = request.Rating;
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success("Rating updated successfully");
        }

        var rate = new Rate
        {
            UserId = currentUserId,
            MovieId = request.MovieId,
            Rating = request.Rating
        };

        _context.Add(rate);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Movie rated successfully");
    }
}
