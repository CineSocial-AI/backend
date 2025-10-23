using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.Rates;

public class RateMovieUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RateMovieUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> ExecuteAsync(int movieId, decimal rating, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User not authenticated");

        if (rating < 0 || rating > 10)
        {
            throw new ArgumentException("Rating must be between 0 and 10");
        }

        var existingRate = await _context.Rates
            .FirstOrDefaultAsync(r => r.MovieId == movieId && r.UserId == currentUserId, cancellationToken);

        if (existingRate != null)
        {
            existingRate.Rating = rating;
            await _context.SaveChangesAsync(cancellationToken);
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

        return true;
    }
}
