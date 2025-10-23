using CineSocial.Application.Features.Blocks.Queries.GetBlockedUsers;
using CineSocial.Application.UseCases.Blocks;
using HotChocolate;
using HotChocolate.Authorization;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class BlockQueries
{
    [Authorize]
    public List<BlockedUserDto> GetBlockedUsers(
        [Service] GetBlockedUsersUseCase useCase)
    {
        return useCase.Execute();
    }
}
