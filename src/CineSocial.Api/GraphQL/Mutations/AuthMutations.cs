using CineSocial.Application.Features.Auth.Commands.Login;
using CineSocial.Application.Features.Auth.Commands.Register;
using CineSocial.Application.UseCases.Auth;
using HotChocolate;

namespace CineSocial.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class AuthMutations
{
    public async Task<LoginResponse> Login(
        string email,
        string password,
        [Service] LoginUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(email, password, cancellationToken);
    }

    public async Task<RegisterResponse> Register(
        string username,
        string email,
        string password,
        [Service] RegisterUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(username, email, password, cancellationToken);
    }
}
