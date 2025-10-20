using CineSocial.Application.Common.Models;
using MediatR;
using System.ComponentModel;

namespace CineSocial.Application.Features.Rates.Queries.GetUserRateForMovie;

public record GetUserRateForMovieQuery(
    [property: DefaultValue(1)] int MovieId
) : IRequest<Result<decimal?>>;
