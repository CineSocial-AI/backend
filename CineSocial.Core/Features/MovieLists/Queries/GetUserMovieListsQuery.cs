using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using MediatR;

namespace CineSocial.Core.Features.MovieLists.Queries;

public record GetUserMovieListsQuery(
    Guid UserId,
    bool IncludeWatchlist = true
) : IRequest<Result<List<MovieListResult>>>;

public record MovieListResult(
    Guid Id,
    Guid UserId,
    string Name,
    string Description,
    bool IsPublic,
    bool IsWatchlist,
    int MovieCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string UserFullName,
    string UserUsername
);

public class GetUserMovieListsQueryHandler : IRequestHandler<GetUserMovieListsQuery, Result<List<MovieListResult>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserMovieListsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<MovieListResult>>> Handle(GetUserMovieListsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get user to verify existence
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result<List<MovieListResult>>.Failure("Kullanıcı bulunamadı.");
            }

            // Build predicate based on whether to include watchlist
            System.Linq.Expressions.Expression<Func<MovieList, bool>> predicate;
            if (request.IncludeWatchlist)
            {
                predicate = ml => ml.UserId == request.UserId;
            }
            else
            {
                predicate = ml => ml.UserId == request.UserId && !ml.IsWatchlist;
            }

            var movieLists = await _unitOfWork.MovieLists.FindAsync(predicate, cancellationToken);

            // Get movie counts for each list
            var listIds = movieLists.Select(ml => ml.Id).ToList();
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
                $"{user.FirstName} {user.LastName}",
                user.Username
            )).OrderBy(ml => ml.CreatedAt).ToList();

            return Result<List<MovieListResult>>.Success(results);
        }
        catch (Exception ex)
        {
            return Result<List<MovieListResult>>.Failure($"Kullanıcı listeleri yüklenirken hata oluştu: {ex.Message}");
        }
    }
}