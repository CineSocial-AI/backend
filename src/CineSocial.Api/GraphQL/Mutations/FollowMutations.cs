using CineSocial.Application.UseCases.Follows;
using HotChocolate;
using HotChocolate.Authorization;

namespace CineSocial.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class FollowMutations
{
    [Authorize]
    public async Task<bool> FollowUser(
        int followingId,
        [Service] FollowUserUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(followingId, cancellationToken);
    }

    [Authorize]
    public async Task<bool> UnfollowUser(
        int followingId,
        [Service] UnfollowUserUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(followingId, cancellationToken);
    }
}
