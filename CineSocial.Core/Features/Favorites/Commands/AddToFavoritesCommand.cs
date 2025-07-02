using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using MediatR;

namespace CineSocial.Core.Features.Favorites.Commands;

public record AddToFavoritesCommand(
    Guid UserId,
    Guid MovieId
) : IRequest<Result<FavoriteResult>>;

public record FavoriteResult(
    Guid Id,
    Guid UserId,
    Guid MovieId,
    DateTime CreatedAt,
    string MovieTitle,
    string MoviePosterPath
);

public class AddToFavoritesCommandHandler : IRequestHandler<AddToFavoritesCommand, Result<FavoriteResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddToFavoritesCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<FavoriteResult>> Handle(AddToFavoritesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if movie exists
            var movie = await _unitOfWork.Movies.GetByIdAsync(request.MovieId, cancellationToken);
            if (movie == null)
            {
                return Result<FavoriteResult>.Failure("Film bulunamadı.");
            }

            // Check if user exists
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result<FavoriteResult>.Failure("Kullanıcı bulunamadı.");
            }

            // Check if already in favorites
            var existingFavorite = await _unitOfWork.Favorites.FirstOrDefaultAsync(
                f => f.UserId == request.UserId && f.MovieId == request.MovieId,
                cancellationToken
            );

            if (existingFavorite != null)
            {
                return Result<FavoriteResult>.Failure("Bu film zaten favorilerinizde.");
            }

            var favorite = new Favorite
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                MovieId = request.MovieId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Favorites.AddAsync(favorite, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new FavoriteResult(
                favorite.Id,
                favorite.UserId,
                favorite.MovieId,
                favorite.CreatedAt,
                movie.Title,
                movie.PosterPath ?? ""
            );

            return Result<FavoriteResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<FavoriteResult>.Failure($"Favorilere eklenirken hata oluştu: {ex.Message}");
        }
    }
}