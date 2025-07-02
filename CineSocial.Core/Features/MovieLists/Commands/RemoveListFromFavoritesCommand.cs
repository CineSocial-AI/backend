using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.MovieLists.Commands;

public record RemoveListFromFavoritesCommand(
    Guid UserId,
    Guid MovieListId
) : IRequest<Result<bool>>;

public class RemoveListFromFavoritesCommandHandler : IRequestHandler<RemoveListFromFavoritesCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveListFromFavoritesCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(RemoveListFromFavoritesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var listFavorite = await _unitOfWork.ListFavorites.FirstOrDefaultAsync(
                lf => lf.UserId == request.UserId && lf.MovieListId == request.MovieListId,
                cancellationToken
            );

            if (listFavorite == null)
            {
                return Result<bool>.Failure("Bu liste favorilerinizde değil.");
            }

            _unitOfWork.ListFavorites.Remove(listFavorite);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Liste favorilerden kaldırılırken hata oluştu: {ex.Message}");
        }
    }
}