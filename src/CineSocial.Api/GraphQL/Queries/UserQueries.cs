using CineSocial.Api.GraphQL;
using CineSocial.Application.Features.Users.Queries.GetCurrent;
using HotChocolate.Authorization;
using MediatR;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class UserQueries
{
    [Authorize]
    public async Task<GetCurrentUserResponse> GetCurrentUser(
        [Service] IMediator mediator,
        [Service] GraphQLUserContextAccessor userContext,
        CancellationToken cancellationToken)
    {
        var userId = userContext.GetCurrentUserId()
            ?? throw new GraphQLException("User not authenticated");

        var query = new GetCurrentUserQuery(userId);
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to get current user");
        }

        return result.Data!;
    }
}
