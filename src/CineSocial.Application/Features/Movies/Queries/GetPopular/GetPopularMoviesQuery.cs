using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Movies.Queries.GetPopular;

public record GetPopularMoviesQuery(int Page = 1, int PageSize = 20) : IRequest<Result<object>>;
