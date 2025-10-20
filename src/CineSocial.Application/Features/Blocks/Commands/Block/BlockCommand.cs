using CineSocial.Application.Common.Models;
using MediatR;
using System.ComponentModel;

namespace CineSocial.Application.Features.Blocks.Commands.Block;

public record BlockCommand(
    [property: DefaultValue(2)] int BlockedUserId
) : IRequest<Result>;
