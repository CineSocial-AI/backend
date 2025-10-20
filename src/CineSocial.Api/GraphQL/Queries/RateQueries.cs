using CineSocial.Application.Features.Rates.Queries.GetMovieRatingStats;
using CineSocial.Application.Features.Rates.Queries.GetUserRateForMovie;
using HotChocolate;
using MediatR;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class RateQueries
{
    public async Task<MovieRatingStatsDto?> GetMovieRatingStats(
        int movieId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetMovieRatingStatsQuery(movieId);
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to get rating stats");
        }

        return result.Data;
    }

    public async Task<decimal?> GetUserRateForMovie(
        int movieId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetUserRateForMovieQuery(movieId);
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to get user rating");
        }

        return result.Data;
    }
}
