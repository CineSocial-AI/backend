using CineSocial.Application.UseCases.Comments;
using CineSocial.Domain.Entities.Social;
using CineSocial.Domain.Enums;
using HotChocolate;
using HotChocolate.Authorization;

namespace CineSocial.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class CommentMutations
{
    [Authorize]
    public async Task<Comment> CreateComment(
        CommentableType commentableType,
        int commentableId,
        string content,
        [Service] CreateCommentUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(commentableType, commentableId, content, cancellationToken);
    }

    [Authorize]
    public async Task<Comment> ReplyToComment(
        int parentCommentId,
        string content,
        [Service] ReplyToCommentUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(parentCommentId, content, cancellationToken);
    }

    [Authorize]
    public async Task<bool> UpdateComment(
        int commentId,
        string content,
        [Service] UpdateCommentUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(commentId, content, cancellationToken);
    }

    [Authorize]
    public async Task<bool> DeleteComment(
        int commentId,
        [Service] DeleteCommentUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(commentId, cancellationToken);
    }
}
