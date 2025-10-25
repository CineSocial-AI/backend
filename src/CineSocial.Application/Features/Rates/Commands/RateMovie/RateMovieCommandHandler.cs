using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CineSocial.Application.Features.Rates.Commands.RateMovie;

public class RateMovieCommandHandler : IRequestHandler<RateMovieCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<RateMovieCommandHandler> _logger;

    public RateMovieCommandHandler(
        IApplicationDbContext context,
        ILogger<RateMovieCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result> Handle(RateMovieCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = 1; // TODO: Get from ICurrentUserService

        _logger.LogInformation("Rating movie: UserId={UserId}, MovieId={MovieId}, Rating={Rating}",
            currentUserId, request.MovieId, request.Rating);

        if (request.Rating < 0 || request.Rating > 10)
        {
            _logger.LogWarning("Invalid rating value: {Rating} for MovieId={MovieId}, UserId={UserId}",
                request.Rating, request.MovieId, currentUserId);
            return Result.Failure("Rating must be between 0 and 10");
        }

        var existingRate = await _context.Rates
            .FirstOrDefaultAsync(r => r.MovieId == request.MovieId && r.UserId == currentUserId, cancellationToken);

        if (existingRate != null)
        {
            var oldRating = existingRate.Rating;
            existingRate.Rating = request.Rating;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Rating updated: UserId={UserId}, MovieId={MovieId}, OldRating={OldRating}, NewRating={NewRating}",
                currentUserId, request.MovieId, oldRating, request.Rating);

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

        _logger.LogInformation("Movie rated successfully: UserId={UserId}, MovieId={MovieId}, Rating={Rating}",
            currentUserId, request.MovieId, request.Rating);

        return Result.Success("Movie rated successfully");
    }
}
