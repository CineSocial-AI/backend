using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Enums;
using MediatR;

namespace CineSocial.Core.Features.Reactions.Commands;

public record DeleteReactionCommand(
    Guid UserId,
    Guid CommentId
) : IRequest<Result<bool>>;

public class DeleteReactionCommandHandler : IRequestHandler<DeleteReactionCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReactionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteReactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var reaction = await _unitOfWork.Reactions.FirstOrDefaultAsync(
                r => r.UserId == request.UserId && r.CommentId == request.CommentId,
                cancellationToken
            );

            if (reaction == null)
            {
                return Result<bool>.Failure("Tepki bulunamadı.");
            }

            // Get comment to update counts
            var comment = await _unitOfWork.Comments.GetByIdAsync(request.CommentId, cancellationToken);
            if (comment != null)
            {
                // Update comment counts
                if (reaction.Type == ReactionType.Upvote)
                    comment.UpvotesCount = Math.Max(0, comment.UpvotesCount - 1);
                else if (reaction.Type == ReactionType.Downvote)
                    comment.DownvotesCount = Math.Max(0, comment.DownvotesCount - 1);

                _unitOfWork.Comments.Update(comment);
            }

            _unitOfWork.Reactions.Remove(reaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Tepki silinirken hata oluştu: {ex.Message}");
        }
    }
}