using CineSocial.Application.UseCases.Comments;
using CineSocial.Domain.Entities.Social;
using HotChocolate;
using HotChocolate.Data;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class CommentQueries
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Comment> GetMovieComments(
        int movieId,
        [Service] GetMovieCommentsUseCase useCase)
    {
        return useCase.Execute(movieId);
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Comment> GetCommentReplies(
        int commentId,
        [Service] GetCommentRepliesUseCase useCase)
    {
        return useCase.Execute(commentId);
    }
}
