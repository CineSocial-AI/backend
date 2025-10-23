using CineSocial.Application.Features.Users.Commands.UpdateProfile;
using CineSocial.Application.UseCases.Users;
using HotChocolate;
using HotChocolate.Authorization;

namespace CineSocial.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserMutations
{
    [Authorize]
    public async Task<UpdateProfileResponse> UpdateProfile(
        string? username,
        string? bio,
        int? profileImageId,
        int? backgroundImageId,
        [Service] UpdateProfileUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(username, bio, profileImageId, backgroundImageId, cancellationToken);
    }
}
