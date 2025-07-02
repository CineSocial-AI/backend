using CineSocial.Core.Features.Favorites.Commands;
using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Favorites.Queries;

public record GetUserFavoritesQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 10,
    string SortBy = "created_at",
    string SortOrder = "desc"
) : IRequest<Result<FavoriteListResult>>;

public record FavoriteListResult(
    List<FavoriteResult> Favorites,
    int TotalCount,
    int TotalPages,
    int CurrentPage,
    int PageSize
);

public class GetUserFavoritesQueryHandler : IRequestHandler<GetUserFavoritesQuery, Result<FavoriteListResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserFavoritesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<FavoriteListResult>> Handle(GetUserFavoritesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if user exists
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result<FavoriteListResult>.Failure("Kullanıcı bulunamadı.");
            }

            // Get favorites with movie information
            var favorites = await _unitOfWork.Favorites.FindAsync(
                f => f.UserId == request.UserId,
                f => f.Movie!
            );

            var favoritesList = favorites.ToList();

            // Apply sorting
            favoritesList = request.SortBy.ToLower() switch
            {
                "movie_title" => request.SortOrder.ToLower() == "asc" 
                    ? favoritesList.OrderBy(f => f.Movie!.Title).ToList()
                    : favoritesList.OrderByDescending(f => f.Movie!.Title).ToList(),
                "movie_release_date" => request.SortOrder.ToLower() == "asc"
                    ? favoritesList.OrderBy(f => f.Movie!.ReleaseDate ?? DateTime.MinValue).ToList()
                    : favoritesList.OrderByDescending(f => f.Movie!.ReleaseDate ?? DateTime.MinValue).ToList(),
                _ => request.SortOrder.ToLower() == "asc"
                    ? favoritesList.OrderBy(f => f.CreatedAt).ToList()
                    : favoritesList.OrderByDescending(f => f.CreatedAt).ToList()
            };

            // Apply pagination
            var totalCount = favoritesList.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var skip = (request.Page - 1) * request.PageSize;
            var pagedFavorites = favoritesList.Skip(skip).Take(request.PageSize).ToList();

            var favoriteResults = pagedFavorites.Select(f => new FavoriteResult(
                f.Id,
                f.UserId,
                f.MovieId,
                f.CreatedAt,
                f.Movie?.Title ?? "",
                f.Movie?.PosterPath ?? ""
            )).ToList();

            var result = new FavoriteListResult(
                favoriteResults,
                totalCount,
                totalPages,
                request.Page,
                request.PageSize
            );

            return Result<FavoriteListResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<FavoriteListResult>.Failure($"Favoriler sorgulanırken hata oluştu: {ex.Message}");
        }
    }
}