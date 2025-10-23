using CineSocial.Application.Features.Follows.Queries.GetFollowers;
using CineSocial.Application.Features.Follows.Queries.GetFollowing;
using CineSocial.Application.UseCases.Follows;
using HotChocolate;
using HotChocolate.Authorization;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class FollowQueries
{
    [Authorize]
    public List<FollowerDto> GetFollowers(
        int userId,
        [Service] GetFollowersUseCase useCase)
    {
        return useCase.Execute(userId);
    }

    [Authorize]
    public List<FollowingDto> GetFollowing(
        int userId,
        [Service] GetFollowingUseCase useCase)
    {
        return useCase.Execute(userId);
    }
}
