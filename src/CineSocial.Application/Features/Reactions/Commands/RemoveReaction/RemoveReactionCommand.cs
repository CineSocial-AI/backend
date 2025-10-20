using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Reactions.Commands.RemoveReaction;

public record RemoveReactionCommand(
    int CommentId
) : IRequest<Result>;
