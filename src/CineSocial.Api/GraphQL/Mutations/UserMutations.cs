using CineSocial.Application.Features.Users.Commands.UpdateProfile;
using HotChocolate.Authorization;
using MediatR;

namespace CineSocial.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserMutations
{
    [Authorize]
    public async Task<UpdateProfileResponse> UpdateProfile(
        int userId,
        string? username,
        string? bio,
        int? profileImageId,
        int? backgroundImageId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProfileCommand(
            userId,
            username,
            bio,
            profileImageId,
            backgroundImageId
        );

        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to update profile");
        }

        return result.Data!;
    }
}
