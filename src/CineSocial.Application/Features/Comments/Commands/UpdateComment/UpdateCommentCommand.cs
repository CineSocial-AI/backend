using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Comments.Commands.UpdateComment;

public record UpdateCommentCommand(
    int CommentId,
    string Content
) : IRequest<Result>;
