using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Ratings.Commands;

public record DeleteRatingCommand(
    Guid UserId,
    Guid MovieId
) : IRequest<Result<bool>>;

public class DeleteRatingCommandHandler : IRequestHandler<DeleteRatingCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRatingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteRatingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var rating = await _unitOfWork.Ratings.FirstOrDefaultAsync(
                r => r.UserId == request.UserId && r.MovieId == request.MovieId,
                cancellationToken
            );

            if (rating == null)
            {
                return Result<bool>.Failure("Puanlama bulunamadı.");
            }

            _unitOfWork.Ratings.Remove(rating);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Puanlama silinirken hata oluştu: {ex.Message}");
        }
    }
}