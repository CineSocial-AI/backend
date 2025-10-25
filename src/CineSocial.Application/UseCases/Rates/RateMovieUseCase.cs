using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CineSocial.Application.UseCases.Rates;

public class RateMovieUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RateMovieUseCase> _logger;

    public RateMovieUseCase(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<RateMovieUseCase> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> ExecuteAsync(int movieId, decimal rating, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        _logger.LogInformation("Rating movie: UserId={UserId}, MovieId={MovieId}, Rating={Rating}",
            currentUserId, movieId, rating);

        if (rating < 0 || rating > 10)
        {
            _logger.LogWarning("Invalid rating value: UserId={UserId}, MovieId={MovieId}, Rating={Rating}",
                currentUserId, movieId, rating);
            throw new ValidationException("rating", "Rating must be between 0 and 10");
        }

        var existingRate = await _context.Rates
            .FirstOrDefaultAsync(r => r.MovieId == movieId && r.UserId == currentUserId, cancellationToken);

        if (existingRate != null)
        {
            var oldRating = existingRate.Rating;
            existingRate.Rating = rating;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Rating updated: UserId={UserId}, MovieId={MovieId}, OldRating={OldRating}, NewRating={NewRating}",
                currentUserId, movieId, oldRating, rating);
            return true;
        }

        var rate = new Rate
        {
            UserId = currentUserId,
            MovieId = movieId,
            Rating = rating
        };

        _context.Add(rate);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Movie rated successfully: UserId={UserId}, MovieId={MovieId}, Rating={Rating}",
            currentUserId, movieId, rating);

        return true;
    }
}
