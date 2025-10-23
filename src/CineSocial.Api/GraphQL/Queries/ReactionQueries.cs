using CineSocial.Domain.Entities.Social;
using CineSocial.Infrastructure.Data;
using HotChocolate;
using HotChocolate.Data;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class ReactionQueries
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Reaction> GetCommentReactions(
        int commentId,
        [Service] ApplicationDbContext context)
    {
        return context.Reactions
            .Where(r => r.CommentId == commentId);
    }

    [UseProjection]
    public async Task<Reaction?> GetReactionById(
        int id,
        [Service] ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        return await context.Reactions.FindAsync(new object[] { id }, cancellationToken);
    }
}
