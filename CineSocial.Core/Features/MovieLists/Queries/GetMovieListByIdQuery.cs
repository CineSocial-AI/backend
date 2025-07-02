using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.MovieLists.Queries;

public record GetMovieListByIdQuery(
    Guid MovieListId,
    Guid? UserId = null
) : IRequest<Result<MovieListDetailResult>>;

public record MovieListDetailResult(
    Guid Id,
    Guid UserId,
    string Name,
    string Description,
    bool IsPublic,
    bool IsWatchlist,
    List<MovieListItemResult> Movies,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string UserFullName,
    string UserUsername,
    bool IsFavorited
);

public record MovieListItemResult(
    Guid Id,
    Guid MovieId,
    string MovieTitle,
    string MoviePosterPath,
    string Notes,
    int Order,
    DateTime AddedAt
);

public class GetMovieListByIdQueryHandler : IRequestHandler<GetMovieListByIdQuery, Result<MovieListDetailResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMovieListByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MovieListDetailResult>> Handle(GetMovieListByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var movieList = await _unitOfWork.MovieLists.GetByIdAsync(request.MovieListId, cancellationToken);
            if (movieList == null)
            {
                return Result<MovieListDetailResult>.Failure("Liste bulunamadı.");
            }

            // Check if user can access this list
            if (!movieList.IsPublic && movieList.UserId != request.UserId)
            {
                return Result<MovieListDetailResult>.Failure("Bu listeye erişim yetkiniz yok.");
            }

            // Get list owner
            var user = await _unitOfWork.Users.GetByIdAsync(movieList.UserId, cancellationToken);
            if (user == null)
            {
                return Result<MovieListDetailResult>.Failure("Liste sahibi bulunamadı.");
            }

            // Get movies in the list
            var movieListItems = await _unitOfWork.MovieListItems.FindAsync(
                mli => mli.MovieListId == request.MovieListId,
                mli => mli.Movie!
            );

            var movieResults = movieListItems
                .OrderBy(mli => mli.Order)
                .Select(mli => new MovieListItemResult(
                    mli.Id,
                    mli.MovieId,
                    mli.Movie?.Title ?? "",
                    mli.Movie?.PosterPath ?? "",
                    mli.Notes ?? "",
                    mli.Order,
                    mli.CreatedAt
                )).ToList();

            // Check if current user has favorited this list
            bool isFavorited = false;
            if (request.UserId.HasValue && request.UserId.Value != movieList.UserId)
            {
                var favorite = await _unitOfWork.ListFavorites.FirstOrDefaultAsync(
                    lf => lf.UserId == request.UserId.Value && lf.MovieListId == request.MovieListId,
                    cancellationToken
                );
                isFavorited = favorite != null;
            }

            var result = new MovieListDetailResult(
                movieList.Id,
                movieList.UserId,
                movieList.Name,
                movieList.Description ?? "",
                movieList.IsPublic,
                movieList.IsWatchlist,
                movieResults,
                movieList.CreatedAt,
                movieList.UpdatedAt,
                $"{user.FirstName} {user.LastName}",
                user.Username,
                isFavorited
            );

            return Result<MovieListDetailResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<MovieListDetailResult>.Failure($"Liste detayları yüklenirken hata oluştu: {ex.Message}");
        }
    }
}