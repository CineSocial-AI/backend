using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using CineSocial.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Reactions.Queries.GetCommentReactions;

public class GetCommentReactionsQueryHandler : IRequestHandler<GetCommentReactionsQuery, Result<object>>
{
    private readonly IRepository<Reaction> _reactionRepository;

    public GetCommentReactionsQueryHandler(IRepository<Reaction> reactionRepository)
    {
        _reactionRepository = reactionRepository;
    }

    public async Task<Result<object>> Handle(GetCommentReactionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var reactions = await _reactionRepository.GetQueryable()
                .Where(r => r.CommentId == request.CommentId)
                .Include(r => r.User)
                .ToListAsync(cancellationToken);

            var summary = new
            {
                total = reactions.Count,
                upvotes = reactions.Count(r => r.Type == ReactionType.Upvote),
                downvotes = reactions.Count(r => r.Type == ReactionType.Downvote),
                reactions
            };

            return Result<object>.Success(summary);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure($"Failed to retrieve comment reactions: {ex.Message}");
        }
    }
}
