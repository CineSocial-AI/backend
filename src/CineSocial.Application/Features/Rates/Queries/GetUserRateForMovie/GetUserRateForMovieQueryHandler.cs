using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Rates.Queries.GetUserRateForMovie;

public class GetUserRateForMovieQueryHandler : IRequestHandler<GetUserRateForMovieQuery, Result<decimal?>>
{
    private readonly IApplicationDbContext _context;

    public GetUserRateForMovieQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Result<decimal?>> Handle(GetUserRateForMovieQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = 1;

        var rate = _context.Rates
            .FirstOrDefault(r => r.MovieId == request.MovieId && r.UserId == currentUserId);

        return Task.FromResult(Result<decimal?>.Success(rate?.Rating));
    }
}
