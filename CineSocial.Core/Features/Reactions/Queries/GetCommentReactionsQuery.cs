using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Enums;
using MediatR;

namespace CineSocial.Core.Features.Reactions.Queries;

public record GetCommentReactionsQuery(Guid CommentId) : IRequest<Result<CommentReactionStats>>;

public record CommentReactionStats(
    Guid CommentId,
    int UpvotesCount,
    int DownvotesCount,
    int TotalReactions
);

public class GetCommentReactionsQueryHandler : IRequestHandler<GetCommentReactionsQuery, Result<CommentReactionStats>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCommentReactionsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CommentReactionStats>> Handle(GetCommentReactionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if comment exists (optional - we can still return stats for deleted comments)
            // var comment = await _unitOfWork.Comments.GetByIdAsync(request.CommentId, cancellationToken);

            var reactions = await _unitOfWork.Reactions.FindAsync(
                r => r.CommentId == request.CommentId,
                cancellationToken
            );

            var reactionsList = reactions.ToList();
            var upvotesCount = reactionsList.Count(r => r.Type == ReactionType.Upvote);
            var downvotesCount = reactionsList.Count(r => r.Type == ReactionType.Downvote);
            var totalReactions = reactionsList.Count;

            var stats = new CommentReactionStats(
                request.CommentId,
                upvotesCount,
                downvotesCount,
                totalReactions
            );

            return Result<CommentReactionStats>.Success(stats);
        }
        catch (Exception ex)
        {
            return Result<CommentReactionStats>.Failure($"Yorum tepkileri sorgulanırken hata oluştu: {ex.Message}");
        }
    }
}