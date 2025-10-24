using CineSocial.Api.GraphQL.Types;
using CineSocial.Application.UseCases.MovieLists;
using HotChocolate;
using HotChocolate.Types;

namespace CineSocial.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class MovieListMutations
{
    public async Task<MovieListType> CreateMovieList(
        string name,
        string? description,
        bool isPublic,
        int? coverImageId,
        [Service] CreateMovieListUseCase useCase,
        CancellationToken cancellationToken)
    {
        var list = await useCase.ExecuteAsync(name, description, isPublic, coverImageId, cancellationToken);
        return MovieListType.FromEntity(list);
    }

    public async Task<bool> UpdateMovieList(
        int id,
        string? name,
        string? description,
        bool? isPublic,
        int? coverImageId,
        [Service] UpdateMovieListUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(id, name, description, isPublic, coverImageId, cancellationToken);
    }

    public async Task<bool> DeleteMovieList(
        int id,
        [Service] DeleteMovieListUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(id, cancellationToken);
    }

    public async Task<bool> AddMovieToList(
        int listId,
        int movieId,
        [Service] AddMovieToListUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(listId, movieId, cancellationToken);
    }

    public async Task<bool> RemoveMovieFromList(
        int listId,
        int movieId,
        [Service] RemoveMovieFromListUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(listId, movieId, cancellationToken);
    }

    public async Task<bool> ReorderMovieInList(
        int listId,
        int movieId,
        int newOrder,
        [Service] ReorderMovieInListUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(listId, movieId, newOrder, cancellationToken);
    }

    public async Task<bool> FavoriteMovieList(
        int listId,
        [Service] FavoriteMovieListUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(listId, cancellationToken);
    }

    public async Task<bool> UnfavoriteMovieList(
        int listId,
        [Service] UnfavoriteMovieListUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(listId, cancellationToken);
    }
}
