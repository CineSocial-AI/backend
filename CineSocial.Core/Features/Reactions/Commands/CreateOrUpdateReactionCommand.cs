using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using CineSocial.Domain.Enums;
using MediatR;

namespace CineSocial.Core.Features.Reactions.Commands;

public record CreateOrUpdateReactionCommand(
    Guid UserId,
    Guid CommentId,
    ReactionType Type
) : IRequest<Result<ReactionResult>>;

public record ReactionResult(
    Guid Id,
    Guid UserId,
    Guid CommentId,
    ReactionType Type,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public class CreateOrUpdateReactionCommandHandler : IRequestHandler<CreateOrUpdateReactionCommand, Result<ReactionResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrUpdateReactionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReactionResult>> Handle(CreateOrUpdateReactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if comment exists
            var comment = await _unitOfWork.Comments.GetByIdAsync(request.CommentId, cancellationToken);
            if (comment == null)
            {
                return Result<ReactionResult>.Failure("Yorum bulunamadı.");
            }

            // Check if user exists
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result<ReactionResult>.Failure("Kullanıcı bulunamadı.");
            }

            // Check if user already has a reaction on this comment
            var existingReaction = await _unitOfWork.Reactions.FirstOrDefaultAsync(
                r => r.UserId == request.UserId && r.CommentId == request.CommentId,
                cancellationToken
            );

            Reaction reaction;
            bool isUpdate = false;

            if (existingReaction != null)
            {
                // Update existing reaction
                var oldType = existingReaction.Type;
                existingReaction.Type = request.Type;
                existingReaction.UpdatedAt = DateTime.UtcNow;
                
                _unitOfWork.Reactions.Update(existingReaction);
                reaction = existingReaction;
                isUpdate = true;

                // Update comment counts
                await UpdateCommentCounts(comment, oldType, request.Type);
            }
            else
            {
                // Create new reaction
                reaction = new Reaction
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    CommentId = request.CommentId,
                    Type = request.Type,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Reactions.AddAsync(reaction, cancellationToken);

                // Update comment counts
                await UpdateCommentCounts(comment, null, request.Type);
            }

            _unitOfWork.Comments.Update(comment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new ReactionResult(
                reaction.Id,
                reaction.UserId,
                reaction.CommentId,
                reaction.Type,
                reaction.CreatedAt,
                reaction.UpdatedAt
            );

            return Result<ReactionResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<ReactionResult>.Failure($"Tepki oluşturulurken hata oluştu: {ex.Message}");
        }
    }

    private async Task UpdateCommentCounts(Comment comment, ReactionType? oldType, ReactionType newType)
    {
        // Remove old reaction count
        if (oldType.HasValue)
        {
            if (oldType.Value == ReactionType.Upvote)
                comment.UpvotesCount = Math.Max(0, comment.UpvotesCount - 1);
            else if (oldType.Value == ReactionType.Downvote)
                comment.DownvotesCount = Math.Max(0, comment.DownvotesCount - 1);
        }

        // Add new reaction count
        if (newType == ReactionType.Upvote)
            comment.UpvotesCount++;
        else if (newType == ReactionType.Downvote)
            comment.DownvotesCount++;
    }
}