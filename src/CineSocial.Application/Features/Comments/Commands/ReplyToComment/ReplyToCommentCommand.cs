using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Comments.Commands.ReplyToComment;

public record ReplyToCommentCommand(
    int ParentCommentId,
    string Content
) : IRequest<Result<int>>;
