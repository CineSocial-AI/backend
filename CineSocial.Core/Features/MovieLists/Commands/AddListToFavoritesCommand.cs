using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Core.Features.MovieLists.Commands;

public record AddListToFavoritesCommand(
    Guid UserId,
    Guid MovieListId
) : IRequest<Result<bool>>;

public class AddListToFavoritesCommandHandler : IRequestHandler<AddListToFavoritesCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddListToFavoritesCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(AddListToFavoritesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if movie list exists and is public
            var movieList = await _unitOfWork.MovieLists.GetByIdAsync(request.MovieListId, cancellationToken);
            if (movieList == null)
            {
                return Result<bool>.Failure("Liste bulunamadı.");
            }

            if (!movieList.IsPublic && movieList.UserId != request.UserId)
            {
                return Result<bool>.Failure("Bu listeyi favorilere ekleyemezsiniz.");
            }

            // Check if user already favorited this list
            var existingFavorite = await _unitOfWork.ListFavorites.FirstOrDefaultAsync(
                lf => lf.UserId == request.UserId && lf.MovieListId == request.MovieListId,
                cancellationToken
            );

            if (existingFavorite != null)
            {
                return Result<bool>.Failure("Bu liste zaten favorilerinizde.");
            }

            var listFavorite = new ListFavorite
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                MovieListId = request.MovieListId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ListFavorites.AddAsync(listFavorite, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Liste favorilere eklenirken hata oluştu: {ex.Message}");
        }
    }
}