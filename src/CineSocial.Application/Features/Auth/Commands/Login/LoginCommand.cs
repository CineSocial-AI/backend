using System.ComponentModel;
using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    [property: DefaultValue("user@cinesocial.com")] string Email,
    [property: DefaultValue("User123!")] string Password
) : IRequest<Result<LoginResponse>>;
