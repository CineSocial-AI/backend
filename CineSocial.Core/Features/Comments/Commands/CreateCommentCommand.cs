using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using MediatR;

namespace CineSocial.Core.Features.Comments.Commands;

public record CreateCommentCommand(
    Guid UserId,
    Guid ReviewId,
    string Content,
    Guid? ParentCommentId = null
) : IRequest<Result<CommentResult>>;

public record CommentResult(
    Guid Id,
    Guid UserId,
    Guid ReviewId,
    string Content,
    int UpvotesCount,
    int DownvotesCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    Guid? ParentCommentId,
    string UserFullName,
    string UserUsername
);

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<CommentResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCommentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CommentResult>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if review exists
            var review = await _unitOfWork.Reviews.GetByIdAsync(request.ReviewId, cancellationToken);
            if (review == null)
            {
                return Result<CommentResult>.Failure("Değerlendirme bulunamadı.");
            }

            // Check if user exists
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result<CommentResult>.Failure("Kullanıcı bulunamadı.");
            }

            // Check if parent comment exists (if provided)
            if (request.ParentCommentId.HasValue)
            {
                var parentComment = await _unitOfWork.Comments.GetByIdAsync(request.ParentCommentId.Value, cancellationToken);
                if (parentComment == null)
                {
                    return Result<CommentResult>.Failure("Üst yorum bulunamadı.");
                }

                // Ensure parent comment belongs to the same review
                if (parentComment.ReviewId != request.ReviewId)
                {
                    return Result<CommentResult>.Failure("Üst yorum bu değerlendirmeye ait değil.");
                }
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ReviewId = request.ReviewId,
                Content = request.Content,
                ParentCommentId = request.ParentCommentId,
                UpvotesCount = 0,
                DownvotesCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Comments.AddAsync(comment, cancellationToken);
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
            return Result<CommentResult>.Failure($"Yorum oluşturulurken hata oluştu: {ex.Message}");
        }
    }
}