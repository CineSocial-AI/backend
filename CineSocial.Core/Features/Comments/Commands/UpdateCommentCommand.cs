using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Comments.Commands;

public record UpdateCommentCommand(
    Guid Id,
    Guid UserId,
    string Content
) : IRequest<Result<CommentResult>>;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Result<CommentResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCommentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CommentResult>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(request.Id, cancellationToken);
            if (comment == null)
            {
                return Result<CommentResult>.Failure("Yorum bulunamadı.");
            }

            // Check if user is the owner of the comment
            if (comment.UserId != request.UserId)
            {
                return Result<CommentResult>.Failure("Bu yorumu güncelleme yetkiniz yok.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(comment.UserId, cancellationToken);
            if (user == null)
            {
                return Result<CommentResult>.Failure("Kullanıcı bulunamadı.");
            }

            comment.Content = request.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Comments.Update(comment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new CommentResult(
                comment.Id,
                comment.UserId,
                comment.ReviewId,
                comment.Content,
                comment.UpvotesCount,
                comment.DownvotesCount,
                comment.CreatedAt,
                comment.UpdatedAt,
                comment.ParentCommentId,
                $"{user.FirstName} {user.LastName}",
                user.Username
            );

            return Result<CommentResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<CommentResult>.Failure($"Yorum güncellenirken hata oluştu: {ex.Message}");
        }
    }
}