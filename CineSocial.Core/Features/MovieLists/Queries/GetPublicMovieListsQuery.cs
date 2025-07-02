using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using MediatR;

namespace CineSocial.Core.Features.MovieLists.Queries;

public record GetPublicMovieListsQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchQuery = null
) : IRequest<Result<PagedMovieListResult>>;

public record PagedMovieListResult(
    List<MovieListResult> MovieLists,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

public class GetPublicMovieListsQueryHandler : IRequestHandler<GetPublicMovieListsQuery, Result<PagedMovieListResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPublicMovieListsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedMovieListResult>> Handle(GetPublicMovieListsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Build predicate for public lists
            System.Linq.Expressions.Expression<Func<MovieList, bool>> predicate = ml => ml.IsPublic;

            if (!string.IsNullOrEmpty(request.SearchQuery))
            {
                var searchQuery = request.SearchQuery.ToLowerInvariant();
                System.Linq.Expressions.Expression<Func<MovieList, bool>> searchPredicate = 
                    ml => ml.Name.ToLower().Contains(searchQuery) || 
                          (ml.Description != null && ml.Description.ToLower().Contains(searchQuery));
                
                predicate = CombinePredicates(predicate, searchPredicate);
            }

            // Get paginated results
            var (movieLists, totalCount) = await _unitOfWork.MovieLists.GetPagedAsync(
                request.Page,
                request.PageSize,
                predicate,
                ml => ml.CreatedAt,
                false, // Descending order (newest first)
                ml => ml.User!
            );

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
            return Result<PagedMovieListResult>.Failure($"Genel listeler yüklenirken hata oluştu: {ex.Message}");
        }
    }

    private static System.Linq.Expressions.Expression<Func<MovieList, bool>> CombinePredicates(
        System.Linq.Expressions.Expression<Func<MovieList, bool>> first,
        System.Linq.Expressions.Expression<Func<MovieList, bool>> second)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(MovieList));
        var leftVisitor = new ReplaceExpressionVisitor(first.Parameters[0], parameter);
        var left = leftVisitor.Visit(first.Body);
        var rightVisitor = new ReplaceExpressionVisitor(second.Parameters[0], parameter);
        var right = rightVisitor.Visit(second.Body);
        return System.Linq.Expressions.Expression.Lambda<Func<MovieList, bool>>(
            System.Linq.Expressions.Expression.AndAlso(left!, right!), parameter);
    }
}

public class ReplaceExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
{
    private readonly System.Linq.Expressions.Expression _oldValue;
    private readonly System.Linq.Expressions.Expression _newValue;

    public ReplaceExpressionVisitor(System.Linq.Expressions.Expression oldValue, System.Linq.Expressions.Expression newValue)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    public override System.Linq.Expressions.Expression? Visit(System.Linq.Expressions.Expression? node)
    {
        return node == _oldValue ? _newValue : base.Visit(node);
    }
}