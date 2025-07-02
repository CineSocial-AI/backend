using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Favorites.Queries;

public record CheckIsFavoriteQuery(
    Guid UserId,
    Guid MovieId
) : IRequest<Result<bool>>;

public class CheckIsFavoriteQueryHandler : IRequestHandler<CheckIsFavoriteQuery, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckIsFavoriteQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(CheckIsFavoriteQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var isFavorite = await _unitOfWork.Favorites.AnyAsync(
                f => f.UserId == request.UserId && f.MovieId == request.MovieId,
                cancellationToken
            );

            return Result<bool>.Success(isFavorite);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Favori durumu sorgulanırken hata oluştu: {ex.Message}");
        }
    }
}