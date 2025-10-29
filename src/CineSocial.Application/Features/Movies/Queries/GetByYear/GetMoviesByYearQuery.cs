using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Movies.Queries.GetByYear;

public record GetMoviesByYearQuery(int Year, int Page = 1, int PageSize = 20) : IRequest<Result<object>>;
