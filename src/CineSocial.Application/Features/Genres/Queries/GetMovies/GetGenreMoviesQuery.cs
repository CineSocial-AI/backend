using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Genres.Queries.GetMovies;

public record GetGenreMoviesQuery(int GenreId, int Page = 1, int PageSize = 20) : IRequest<Result<object>>;
