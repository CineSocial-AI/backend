using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Movies.Queries.GetByPerson;

public record GetMoviesByPersonQuery(int PersonId, int Page = 1, int PageSize = 20) : IRequest<Result<object>>;
