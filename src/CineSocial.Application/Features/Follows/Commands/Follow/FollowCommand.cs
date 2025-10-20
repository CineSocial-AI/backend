using CineSocial.Application.Common.Models;
using MediatR;
using System.ComponentModel;

namespace CineSocial.Application.Features.Follows.Commands.Follow;

public record FollowCommand(
    [property: DefaultValue(2)] int FollowingId
) : IRequest<Result>;
