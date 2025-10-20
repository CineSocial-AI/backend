using CineSocial.Application.Features.Rates.Commands.RateMovie;
using CineSocial.Application.Features.Rates.Commands.RemoveRate;
using HotChocolate.Authorization;
using MediatR;

namespace CineSocial.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class RateMutations
{
    [Authorize]
    public async Task<bool> RateMovie(
        int movieId,
        decimal rating,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new RateMovieCommand(movieId, rating);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to rate movie");
        }

        return true;
    }

    [Authorize]
    public async Task<bool> RemoveRate(
        int movieId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new RemoveRateCommand(movieId);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to remove rating");
        }

        return true;
    }
}
