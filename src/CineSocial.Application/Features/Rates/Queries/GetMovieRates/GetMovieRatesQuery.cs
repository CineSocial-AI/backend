using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Rates.Queries.GetMovieRates;

public record GetMovieRatesQuery(int MovieId, int Page = 1, int PageSize = 20) : IRequest<Result<object>>;
