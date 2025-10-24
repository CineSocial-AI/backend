using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;

namespace CineSocial.Application.UseCases.Rates;

public class GetUserRateForMovieUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUserRateForMovieUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<decimal?> ExecuteAsync(int movieId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        var rate = _context.Rates
            .FirstOrDefault(r => r.MovieId == movieId && r.UserId == currentUserId);

        return rate?.Rating;
    }
}
