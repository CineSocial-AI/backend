using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Movies.Queries.GetAll;

public record GetAllMoviesQuery : IRequest<Result<PagedResult<MovieDto>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SearchTerm { get; init; }
    public string? SortBy { get; init; } = "Popularity";
    public bool SortDescending { get; init; } = true;
}
