using CineSocial.Application.Features.Comments.Commands.CreateComment;
using CineSocial.Application.Features.Comments.Commands.DeleteComment;
using CineSocial.Application.Features.Comments.Commands.ReplyToComment;
using CineSocial.Application.Features.Comments.Commands.UpdateComment;
using CineSocial.Domain.Enums;
using HotChocolate.Authorization;
using MediatR;

namespace CineSocial.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class CommentMutations
{
    [Authorize]
    public async Task<int> CreateComment(
        CommentableType commentableType,
        int commentableId,
        string content,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateCommentCommand(commentableType, commentableId, content);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to create comment");
        }

        return result.Data;
    }

    [Authorize]
    public async Task<int> ReplyToComment(
        int parentCommentId,
        string content,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ReplyToCommentCommand(parentCommentId, content);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to reply to comment");
        }

        return result.Data;
    }

    [Authorize]
    public async Task<bool> UpdateComment(
        int commentId,
        string content,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCommentCommand(commentId, content);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to update comment");
        }

        return true;
    }

    [Authorize]
    public async Task<bool> DeleteComment(
        int commentId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCommentCommand(commentId);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to delete comment");
        }

        return true;
    }
}
