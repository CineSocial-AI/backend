using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Reviews.Commands;

public record UpdateReviewCommand(
    Guid Id,
    Guid UserId,
    string Title,
    string Content,
    bool ContainsSpoilers = false
) : IRequest<Result<ReviewResult>>;

public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, Result<ReviewResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReviewCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReviewResult>> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(request.Id, cancellationToken);
            if (review == null)
            {
                return Result<ReviewResult>.Failure("Değerlendirme bulunamadı.");
            }

            // Check if user is the owner of the review
            if (review.UserId != request.UserId)
            {
                return Result<ReviewResult>.Failure("Bu değerlendirmeyi güncelleme yetkiniz yok.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(review.UserId, cancellationToken);
            if (user == null)
            {
                return Result<ReviewResult>.Failure("Kullanıcı bulunamadı.");
            }

            review.Title = request.Title;
            review.Content = request.Content;
            review.ContainsSpoilers = request.ContainsSpoilers;
            review.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new ReviewResult(
                review.Id,
                review.UserId,
                review.MovieId,
                review.Title,
                review.Content,
                review.ContainsSpoilers,
                review.LikesCount,
                review.CreatedAt,
                review.UpdatedAt,
                $"{user.FirstName} {user.LastName}",
                user.Username
            );

            return Result<ReviewResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<ReviewResult>.Failure($"Değerlendirme güncellenirken hata oluştu: {ex.Message}");
        }
    }
}