using CineSocial.Application.Features.Blocks.Queries.GetBlockedUsers;
using HotChocolate.Authorization;
using MediatR;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class BlockQueries
{
    [Authorize]
    public async Task<List<BlockedUserDto>> GetBlockedUsers(
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetBlockedUsersQuery();
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to get blocked users");
        }

        return result.Data ?? new List<BlockedUserDto>();
    }
}
