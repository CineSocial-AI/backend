using CineSocial.Application.Features.Blocks.Commands.Block;
using CineSocial.Application.Features.Blocks.Commands.Unblock;
using HotChocolate.Authorization;
using MediatR;

namespace CineSocial.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class BlockMutations
{
    [Authorize]
    public async Task<bool> BlockUser(
        int blockedUserId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new BlockCommand(blockedUserId);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to block user");
        }

        return true;
    }

    [Authorize]
    public async Task<bool> UnblockUser(
        int blockedUserId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UnblockCommand(blockedUserId);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to unblock user");
        }

        return true;
    }
}
