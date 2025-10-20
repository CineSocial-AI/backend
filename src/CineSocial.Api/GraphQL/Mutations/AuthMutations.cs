using CineSocial.Application.Common.Models;
using CineSocial.Application.Features.Auth.Commands.Login;
using CineSocial.Application.Features.Auth.Commands.Register;
using MediatR;

namespace CineSocial.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class AuthMutations
{
    public async Task<LoginResponse> Login(
        string email,
        string password,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(email, password);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Login failed");
        }

        return result.Data!;
    }

    public async Task<RegisterResponse> Register(
        string username,
        string email,
        string password,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(username, email, password);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Registration failed");
        }

        return result.Data!;
    }
}
