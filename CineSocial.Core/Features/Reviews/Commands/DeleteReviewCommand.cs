using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Reviews.Commands;

public record DeleteReviewCommand(
    Guid Id,
    Guid UserId
) : IRequest<Result<bool>>;

public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReviewCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(request.Id, cancellationToken);
            if (review == null)
            {
                return Result<bool>.Failure("Değerlendirme bulunamadı.");
            }

            // Check if user is the owner of the review
            if (review.UserId != request.UserId)
            {
                return Result<bool>.Failure("Bu değerlendirmeyi silme yetkiniz yok.");
            }

            _unitOfWork.Reviews.Remove(review);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Değerlendirme silinirken hata oluştu: {ex.Message}");
        }
    }
}