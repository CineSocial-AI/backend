using CineSocial.Application.Common.Models;
using MediatR;
using System.ComponentModel;

namespace CineSocial.Application.Features.Follows.Commands.Unfollow;

public record UnfollowCommand(
    [property: DefaultValue(2)] int FollowingId
) : IRequest<Result>;
