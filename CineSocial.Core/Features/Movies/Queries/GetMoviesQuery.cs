using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Core.Features.Movies.Queries;

public record GetMoviesQuery(
    string? Query = null,
    List<Guid>? GenreIds = null,
    int? Year = null,
    int Page = 1,
    int PageSize = 10,
    string SortBy = "popularity",
    string SortOrder = "desc",
    bool IsUpcoming = false
) : IRequest<Result<MovieListResult>>;

public record MovieListResult(
    List<MovieResult> Movies,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

public record MovieResult(
    Guid Id,
    string Title,
    string OriginalTitle,
    string Overview,
    DateTime ReleaseDate,
    int Runtime,
    decimal VoteAverage,
    int VoteCount,
    string Language,
    decimal Popularity,
    string Status,
    long Budget,
    long Revenue,
    string? Tagline,
    List<GenreResult> Genres
);

public record GenreResult(
    Guid Id,
    string Name,
    string Description
);

public class GetMoviesQueryHandler : IRequestHandler<GetMoviesQuery, Result<MovieListResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMoviesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MovieListResult>> Handle(GetMoviesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate pagination parameters
            if (request.Page < 1)
            {
                return Result<MovieListResult>.Failure("Sayfa numarası 1'den küçük olamaz.");
            }

            if (request.PageSize < 1)
            {
                return Result<MovieListResult>.Failure("Sayfa boyutu 1'den küçük olamaz.");
            }

            if (request.PageSize > 50)
            {
                return Result<MovieListResult>.Failure("Sayfa boyutu 50'den büyük olamaz.");
            }
            // Build predicate
            System.Linq.Expressions.Expression<Func<Movie, bool>>? predicate = null;

            if (!string.IsNullOrEmpty(request.Query))
            {
                var searchQuery = request.Query.ToLowerInvariant();
                predicate = m => m.Title.ToLower().Contains(searchQuery) || 
                               (m.OriginalTitle != null && m.OriginalTitle.ToLower().Contains(searchQuery)) ||
                               (m.Overview != null && m.Overview.ToLower().Contains(searchQuery));
            }

            if (request.Year.HasValue)
            {
                System.Linq.Expressions.Expression<Func<Movie, bool>> yearPredicate = m => m.ReleaseDate.HasValue && m.ReleaseDate.Value.Year == request.Year.Value;
                predicate = predicate == null ? yearPredicate : CombinePredicates(predicate, yearPredicate);
            }

            if (request.IsUpcoming)
            {
                var currentDate = DateTime.UtcNow.Date;
                System.Linq.Expressions.Expression<Func<Movie, bool>> upcomingPredicate = m => m.ReleaseDate.HasValue && m.ReleaseDate.Value.Date > currentDate;
                predicate = predicate == null ? upcomingPredicate : CombinePredicates(predicate, upcomingPredicate);
            }

            // Determine sort expression
            System.Linq.Expressions.Expression<Func<Movie, object>> orderBy = request.SortBy.ToLowerInvariant() switch
            {
                "title" => m => m.Title,
                "release_date" => m => m.ReleaseDate,
                "vote_average" => m => m.VoteAverage,
                "popularity" => m => m.Popularity,
                _ => m => m.Popularity
            };

            var ascending = request.SortOrder.ToLowerInvariant() == "asc";

            // Get movies without Genre includes first
            var (movies, totalCount) = await _unitOfWork.Movies.GetPagedAsync(
                request.Page,
                request.PageSize,
                predicate,
                orderBy,
                ascending,
                cancellationToken
            );

            // Load MovieGenres with Genres separately
            var movieIds = movies.Select(m => m.Id).ToList();
            var movieGenres = await _unitOfWork.MovieGenres.FindAsync(
                mg => movieIds.Contains(mg.MovieId),
                mg => mg.Genre!
            );

            // Map genres to movies
            var movieGenresMap = movieGenres.GroupBy(mg => mg.MovieId).ToDictionary(g => g.Key, g => g.ToList());

            var movieResults = movies.Select(m => new MovieResult(
                m.Id,
                m.Title,
                m.OriginalTitle ?? "",
                m.Overview ?? "",
                m.ReleaseDate ?? DateTime.MinValue,
                m.Runtime ?? 0,
                m.VoteAverage ?? 0,
                m.VoteCount ?? 0,
                m.Language ?? "",
                m.Popularity ?? 0,
                m.Status ?? "",
                m.Budget ?? 0,
                m.Revenue ?? 0,
                m.Tagline,
                movieGenresMap.TryGetValue(m.Id, out var genresList) 
                    ? genresList.Select(mg => new GenreResult(
                        mg.Genre!.Id,
                        mg.Genre.Name,
                        mg.Genre.Description ?? ""
                    )).ToList()
                    : new List<GenreResult>()
            )).ToList();

            // Apply genre filter after loading (if needed)
            if (request.GenreIds?.Any() == true)
            {
                movieResults = movieResults
                    .Where(m => m.Genres.Any(g => request.GenreIds.Contains(g.Id)))
                    .ToList();
                totalCount = movieResults.Count;
            }

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            var result = new MovieListResult(
                movieResults,
                totalCount,
                request.Page,
                request.PageSize,
                totalPages
            );

            return Result<MovieListResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<MovieListResult>.Failure($"Filmler yüklenirken hata oluştu: {ex.Message}");
        }
    }

    private static System.Linq.Expressions.Expression<Func<Movie, bool>> CombinePredicates(
        System.Linq.Expressions.Expression<Func<Movie, bool>> first,
        System.Linq.Expressions.Expression<Func<Movie, bool>> second)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(Movie));
        var leftVisitor = new ReplaceExpressionVisitor(first.Parameters[0], parameter);
        var left = leftVisitor.Visit(first.Body);
        var rightVisitor = new ReplaceExpressionVisitor(second.Parameters[0], parameter);
        var right = rightVisitor.Visit(second.Body);
        return System.Linq.Expressions.Expression.Lambda<Func<Movie, bool>>(
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