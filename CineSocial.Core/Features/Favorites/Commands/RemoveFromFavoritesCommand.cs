using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Favorites.Commands;

public record RemoveFromFavoritesCommand(
    Guid UserId,
    Guid MovieId
) : IRequest<Result<bool>>;

public class RemoveFromFavoritesCommandHandler : IRequestHandler<RemoveFromFavoritesCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveFromFavoritesCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(RemoveFromFavoritesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var favorite = await _unitOfWork.Favorites.FirstOrDefaultAsync(
                f => f.UserId == request.UserId && f.MovieId == request.MovieId,
                cancellationToken
            );

            if (favorite == null)
            {
                return Result<bool>.Failure("Bu film favorilerinizde bulunamadı.");
            }

            _unitOfWork.Favorites.Remove(favorite);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Favorilerden çıkarılırken hata oluştu: {ex.Message}");
        }
    }
}