using CineSocial.Api.GraphQL.Types;
using CineSocial.Application.UseCases.MovieLists;
using HotChocolate;
using HotChocolate.Types;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class MovieListQueries
{
    public async Task<MovieListType> MovieList(
        int id,
        [Service] GetMovieListByIdUseCase useCase,
        CancellationToken cancellationToken)
    {
        var list = await useCase.ExecuteAsync(id, cancellationToken);
        return MovieListType.FromEntity(list);
    }

    public async Task<List<MovieListType>> MyMovieLists(
        [Service] GetUserMovieListsUseCase useCase,
        [Service] IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            throw new Exception("User not authenticated");

        var lists = await useCase.ExecuteAsync(userId, cancellationToken);
        return lists.Select(MovieListType.FromEntity).ToList();
    }

    public async Task<List<MovieListType>> UserMovieLists(
        int userId,
        [Service] GetUserMovieListsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var lists = await useCase.ExecuteAsync(userId, cancellationToken);
        return lists.Select(MovieListType.FromEntity).ToList();
    }

    public async Task<List<MovieListType>> PublicMovieLists(
        int skip,
        int take,
        [Service] GetPublicMovieListsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var lists = await useCase.ExecuteAsync(skip, take, cancellationToken);
        return lists.Select(MovieListType.FromEntity).ToList();
    }

    public async Task<MovieListType> MyWatchlist(
        [Service] GetUserWatchlistUseCase useCase,
        CancellationToken cancellationToken)
    {
        var watchlist = await useCase.ExecuteAsync(cancellationToken);
        return MovieListType.FromEntity(watchlist);
    }

    public async Task<List<MovieListType>> MyFavoriteLists(
        [Service] GetUserFavoriteListsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var lists = await useCase.ExecuteAsync(cancellationToken);
        return lists.Select(MovieListType.FromEntity).ToList();
    }
}
