using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using CineSocial.Domain.Enums;
using HotChocolate;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class CommentQueries
{
    /// <summary>
    /// Get all root comments for a movie (no replies, depth = 0)
    /// </summary>
    [UseProjection]
    public IQueryable<Comment> GetMovieComments(
        int movieId,
        [Service] IRepository<Comment> repository)
    {
        return repository.GetQueryable()
            .Where(c => c.CommentableId == movieId &&
                       c.CommentableType == CommentableType.Movie &&
                       c.ParentCommentId == null &&
                       c.DeletedAt == null)
            .Include(c => c.User)
            .Include(c => c.Replies)
                .ThenInclude(r => r.User)
            .Include(c => c.Reactions)
            .OrderByDescending(c => c.CreatedAt);
    }

    /// <summary>
    /// Get all replies for a specific comment
    /// </summary>
    [UseProjection]
    public IQueryable<Comment> GetCommentReplies(
        int commentId,
        [Service] IRepository<Comment> repository)
    {
        return repository.GetQueryable()
            .Where(c => c.ParentCommentId == commentId && c.DeletedAt == null)
            .Include(c => c.User)
            .Include(c => c.Reactions)
            .OrderBy(c => c.CreatedAt);
    }

    /// <summary>
    /// Get a single comment by ID with all its details
    /// </summary>
    public async Task<Comment?> GetComment(
        int id,
        [Service] IRepository<Comment> repository,
        CancellationToken cancellationToken)
    {
        return await repository.GetQueryable()
            .Include(c => c.User)
            .Include(c => c.Replies)
                .ThenInclude(r => r.User)
            .Include(c => c.Reactions)
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null, cancellationToken);
    }

    /// <summary>
    /// Get all comments by a user
    /// </summary>
    [UseProjection]
    public IQueryable<Comment> GetUserComments(
        int userId,
        [Service] IRepository<Comment> repository)
    {
        return repository.GetQueryable()
            .Where(c => c.UserId == userId && c.DeletedAt == null)
            .Include(c => c.User)
            .OrderByDescending(c => c.CreatedAt);
    }
}
