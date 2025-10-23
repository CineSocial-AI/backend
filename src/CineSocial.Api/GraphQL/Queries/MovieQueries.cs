using CineSocial.Application.UseCases.Movies;
using CineSocial.Domain.Entities.Movie;
using HotChocolate;
using HotChocolate.Data;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class MovieQueries
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<MovieEntity> GetMovies(
        [Service] GetMoviesUseCase useCase,
        string? searchTerm = null)
    {
        return useCase.Execute(searchTerm);
    }

    [UseProjection]
    public async Task<MovieEntity?> GetMovieById(
        int id,
        [Service] GetMovieByIdUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(id, cancellationToken);
    }
}
