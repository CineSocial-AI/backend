using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.MovieLists.Queries;

public record GetUserFavoriteListsQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedMovieListResult>>;

public class GetUserFavoriteListsQueryHandler : IRequestHandler<GetUserFavoriteListsQuery, Result<PagedMovieListResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserFavoriteListsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedMovieListResult>> Handle(GetUserFavoriteListsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get user's favorite lists
            var favoriteListIds = await _unitOfWork.ListFavorites.FindAsync(
                lf => lf.UserId == request.UserId,
                cancellationToken
            );

            var listIds = favoriteListIds.Select(lf => lf.MovieListId).ToList();

            if (!listIds.Any())
            {
                var emptyResult = new PagedMovieListResult(
                    new List<MovieListResult>(),
                    0,
                    request.Page,
                    request.PageSize,
                    0
                );
                return Result<PagedMovieListResult>.Success(emptyResult);
            }

            // Get the movie lists
            var (movieLists, totalCount) = await _unitOfWork.MovieLists.GetPagedAsync(
                request.Page,
                request.PageSize,
                ml => listIds.Contains(ml.Id),
                ml => ml.CreatedAt,
                false, // Descending order (newest first)
                ml => ml.User!
            );

            // Get movie counts for each list
            var movieListItems = await _unitOfWork.MovieListItems.FindAsync(
                mli => listIds.Contains(mli.MovieListId),
                cancellationToken
            );

            var movieCounts = movieListItems
                .GroupBy(mli => mli.MovieListId)
                .ToDictionary(g => g.Key, g => g.Count());

            var results = movieLists.Select(ml => new MovieListResult(
                ml.Id,
                ml.UserId,
                ml.Name,
                ml.Description ?? "",
                ml.IsPublic,
                ml.IsWatchlist,
                movieCounts.GetValueOrDefault(ml.Id, 0),
                ml.CreatedAt,
                ml.UpdatedAt,
                $"{ml.User?.FirstName} {ml.User?.LastName}",
                ml.User?.Username ?? ""
            )).ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            var pagedResult = new PagedMovieListResult(
                results,
                totalCount,
                request.Page,
                request.PageSize,
                totalPages
            );

            return Result<PagedMovieListResult>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedMovieListResult>.Failure($"Favori listeler yüklenirken hata oluştu: {ex.Message}");
        }
    }
}