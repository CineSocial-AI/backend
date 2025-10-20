using CineSocial.Application.Common.Models;
using CineSocial.Domain.Enums;
using MediatR;

namespace CineSocial.Application.Features.Comments.Commands.CreateComment;

public record CreateCommentCommand(
    CommentableType CommentableType,
    int CommentableId,
    string Content
) : IRequest<Result<int>>;
