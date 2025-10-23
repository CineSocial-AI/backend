using CineSocial.Application.Features.Users.Queries.GetCurrent;
using CineSocial.Application.UseCases.Users;
using HotChocolate;
using HotChocolate.Authorization;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class UserQueries
{
    [Authorize]
    public async Task<GetCurrentUserResponse> GetCurrentUser(
        [Service] GetCurrentUserUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(cancellationToken);
    }
}
