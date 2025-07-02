using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Comments.Commands;

public record DeleteCommentCommand(
    Guid Id,
    Guid UserId
) : IRequest<Result<bool>>;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCommentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(request.Id, cancellationToken);
            if (comment == null)
            {
                return Result<bool>.Failure("Yorum bulunamadı.");
            }

            // Check if user is the owner of the comment
            if (comment.UserId != request.UserId)
            {
                return Result<bool>.Failure("Bu yorumu silme yetkiniz yok.");
            }

            // Delete child comments first due to DeleteBehavior.Restrict
            var childComments = await _unitOfWork.Comments.FindAsync(
                c => c.ParentCommentId == request.Id,
                cancellationToken
            );

            foreach (var childComment in childComments)
            {
                _unitOfWork.Comments.Remove(childComment);
            }

            // Delete the parent comment
            _unitOfWork.Comments.Remove(comment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Yorum silinirken hata oluştu: {ex.Message}");
        }
    }
}