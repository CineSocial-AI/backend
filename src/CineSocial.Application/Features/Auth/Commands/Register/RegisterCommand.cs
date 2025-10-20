using System.ComponentModel;
using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    [property: DefaultValue("newuser")] string Username,
    [property: DefaultValue("newuser@example.com")] string Email,
    [property: DefaultValue("Password123!")] string Password
) : IRequest<Result<RegisterResponse>>;
