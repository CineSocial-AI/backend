using CineSocial.Application.Features.Rates.Queries.GetMovieRatingStats;
using CineSocial.Application.UseCases.Rates;
using HotChocolate;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class RateQueries
{
    public async Task<MovieRatingStatsDto> GetMovieRatingStats(
        int movieId,
        [Service] GetMovieRatingStatsUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(movieId, cancellationToken);
    }

    public async Task<decimal?> GetUserRateForMovie(
        int movieId,
        [Service] GetUserRateForMovieUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(movieId, cancellationToken);
    }
}
