using CineSocial.Application.Common.Models;
using MediatR;
using System.ComponentModel;

namespace CineSocial.Application.Features.Follows.Queries.GetFollowing;

public record GetFollowingQuery(
    [property: DefaultValue(1)] int UserId,
    [property: DefaultValue(1)] int PageNumber = 1,
    [property: DefaultValue(10)] int PageSize = 10
) : IRequest<Result<List<FollowingDto>>>;
