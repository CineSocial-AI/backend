using CineSocial.Application.Common.Models;
using CineSocial.Application.Features.Movies.Queries.GetAll;
using CineSocial.Application.Features.Movies.Queries.GetById;
using HotChocolate;
using MediatR;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class MovieQueries
{
    public async Task<PagedResult<MovieDto>> GetMovies(
        int page = 1,
        int pageSize = 20,
        string? searchTerm = null,
        string? sortBy = "Popularity",
        bool sortDescending = true,
        [Service] IMediator mediator = default!,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllMoviesQuery
        {
            Page = page,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to get movies");
        }

        return result.Data!;
    }

    public async Task<MovieDetailDto?> GetMovieById(
        int id,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetMovieByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new GraphQLException(result.Errors?.FirstOrDefault() ?? "Failed to get movie");
        }

        return result.Data;
    }
}
