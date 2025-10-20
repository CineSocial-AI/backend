using CineSocial.Application.Features.Follows.Queries.GetFollowers;
using CineSocial.Application.Features.Follows.Queries.GetFollowing;
using HotChocolate.Authorization;
using MediatR;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class FollowQueries
{
    [Authorize]
    public async Task<List<FollowerDto>> GetFollowers(
        int userId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetFollowersQuery(userId);
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to get followers");
        }

        return result.Data ?? new List<FollowerDto>();
    }

    [Authorize]
    public async Task<List<FollowingDto>> GetFollowing(
        int userId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetFollowingQuery(userId);
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to get following");
        }

        return result.Data ?? new List<FollowingDto>();
    }
}
