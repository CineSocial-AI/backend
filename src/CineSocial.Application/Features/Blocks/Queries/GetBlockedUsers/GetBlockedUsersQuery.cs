using CineSocial.Application.Common.Models;
using MediatR;
using System.ComponentModel;

namespace CineSocial.Application.Features.Blocks.Queries.GetBlockedUsers;

public record GetBlockedUsersQuery(
    [property: DefaultValue(1)] int PageNumber = 1,
    [property: DefaultValue(10)] int PageSize = 10
) : IRequest<Result<List<BlockedUserDto>>>;
