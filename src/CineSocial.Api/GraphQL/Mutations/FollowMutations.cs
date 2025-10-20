using CineSocial.Application.Features.Follows.Commands.Follow;
using CineSocial.Application.Features.Follows.Commands.Unfollow;
using HotChocolate.Authorization;
using MediatR;

namespace CineSocial.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class FollowMutations
{
    [Authorize]
    public async Task<bool> FollowUser(
        int followingId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new FollowCommand(followingId);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to follow user");
        }

        return true;
    }

    [Authorize]
    public async Task<bool> UnfollowUser(
        int followingId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UnfollowCommand(followingId);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to unfollow user");
        }

        return true;
    }
}
