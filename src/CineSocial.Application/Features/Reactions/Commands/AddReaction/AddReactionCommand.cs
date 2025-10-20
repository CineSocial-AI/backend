using CineSocial.Application.Common.Models;
using CineSocial.Domain.Enums;
using MediatR;

namespace CineSocial.Application.Features.Reactions.Commands.AddReaction;

public record AddReactionCommand(
    int CommentId,
    ReactionType Type
) : IRequest<Result>;
