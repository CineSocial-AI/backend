using CineSocial.Application.UseCases.Blocks;
using HotChocolate;
using HotChocolate.Authorization;

namespace CineSocial.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class BlockMutations
{
    [Authorize]
    public async Task<bool> BlockUser(
        int blockedUserId,
        [Service] BlockUserUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(blockedUserId, cancellationToken);
    }

    [Authorize]
    public async Task<bool> UnblockUser(
        int blockedUserId,
        [Service] UnblockUserUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(blockedUserId, cancellationToken);
    }
}
