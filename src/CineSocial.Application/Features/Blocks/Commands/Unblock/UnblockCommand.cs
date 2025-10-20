using CineSocial.Application.Common.Models;
using MediatR;
using System.ComponentModel;

namespace CineSocial.Application.Features.Blocks.Commands.Unblock;

public record UnblockCommand(
    [property: DefaultValue(2)] int BlockedUserId
) : IRequest<Result>;
