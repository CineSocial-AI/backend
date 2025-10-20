using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Comments.Commands.DeleteComment;

public record DeleteCommentCommand(
    int CommentId
) : IRequest<Result>;
