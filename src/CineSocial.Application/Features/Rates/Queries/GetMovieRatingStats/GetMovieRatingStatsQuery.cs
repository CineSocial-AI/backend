using CineSocial.Application.Common.Models;
using MediatR;
using System.ComponentModel;

namespace CineSocial.Application.Features.Rates.Queries.GetMovieRatingStats;

public record GetMovieRatingStatsQuery(
    [property: DefaultValue(1)] int MovieId
) : IRequest<Result<MovieRatingStatsDto>>;
