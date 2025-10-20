using CineSocial.Application.Features.Reactions.Commands.AddReaction;
using CineSocial.Application.Features.Reactions.Commands.RemoveReaction;
using CineSocial.Domain.Enums;
using HotChocolate.Authorization;
using MediatR;

namespace CineSocial.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class ReactionMutations
{
    [Authorize]
    public async Task<bool> AddReaction(
        int commentId,
        ReactionType type,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new AddReactionCommand(commentId, type);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to add reaction");
        }

        return true;
    }

    [Authorize]
    public async Task<bool> RemoveReaction(
        int commentId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new RemoveReactionCommand(commentId);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to remove reaction");
        }

        return true;
    }
}
