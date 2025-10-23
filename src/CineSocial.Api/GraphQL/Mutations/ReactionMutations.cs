using CineSocial.Application.UseCases.Reactions;
using CineSocial.Domain.Enums;
using HotChocolate;
using HotChocolate.Authorization;

namespace CineSocial.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class ReactionMutations
{
    [Authorize]
    public async Task<bool> AddReaction(
        int commentId,
        ReactionType type,
        [Service] AddReactionUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(commentId, type, cancellationToken);
    }

    [Authorize]
    public async Task<bool> RemoveReaction(
        int commentId,
        [Service] RemoveReactionUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(commentId, cancellationToken);
    }
}
