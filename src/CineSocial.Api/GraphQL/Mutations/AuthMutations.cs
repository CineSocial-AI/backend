using CineSocial.Api.GraphQL.Payloads;
using CineSocial.Application.Features.Auth.Commands.Login;
using CineSocial.Application.Features.Auth.Commands.Register;
using HotChocolate;
using MediatR;

namespace CineSocial.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class AuthMutations
{
    public async Task<LoginPayload> Login(
        string email,
        string password,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(email, password);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            var
                errorMessage = result.Message ?? "Login failed";
            if (errorMessage.Contains("email") || errorMessage.Contains("password"))
            {
                throw new CineSocial.Application.Common.Exceptions.UnauthorizedException(errorMessage);
            }
            throw new Exception(errorMessage);
        }

        return new LoginPayload
        {
            Token = result.Data!.Token,
            UserId = result.Data.UserId,
            Username = result.Data.Username,
            Email = result.Data.Email,
            Role = result.Data.Role
        };
    }

    public async Task<RegisterPayload> Register(
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
            var errorMessage = result.Message ?? "Registration failed";
            if (errorMessage.Contains("Email already") || errorMessage.Contains("Username already"))
            {
                throw new CineSocial.Application.Common.Exceptions.ConflictException(errorMessage);
            }
            throw new CineSocial.Application.Common.Exceptions.ValidationException(errorMessage);
        }

        return new RegisterPayload
        {
            UserId = result.Data!.UserId,
            Username = result.Data.Username,
            Email = result.Data.Email,
            Role = result.Data.Role
        };
    }
}
