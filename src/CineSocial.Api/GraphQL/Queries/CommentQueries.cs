using CineSocial.Application.Common.Models;
using CineSocial.Application.Features.Comments.Queries.GetCommentById;
using CineSocial.Application.Features.Comments.Queries.GetCommentReplies;
using CineSocial.Application.Features.Comments.Queries.GetMovieComments;
using HotChocolate;
using MediatR;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class CommentQueries
{
    public async Task<PagedResult<CommentDto>> GetMovieComments(
        int movieId,
        int page = 1,
        int pageSize = 10,
        [Service] IMediator mediator = default!,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMovieCommentsQuery(movieId, page, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to get comments");
        }

        return result.Data!;
    }

    public async Task<PagedResult<CommentDto>> GetCommentReplies(
        int commentId,
        int page = 1,
        int pageSize = 10,
        [Service] IMediator mediator = default!,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCommentRepliesQuery(commentId, page, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to get replies");
        }

        return result.Data!;
    }

    public async Task<CommentDto?> GetCommentById(
        int commentId,
        [Service] IMediator mediator = default!,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCommentByIdQuery(commentId);
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to get comment");
        }

        return result.Data;
    }
}
