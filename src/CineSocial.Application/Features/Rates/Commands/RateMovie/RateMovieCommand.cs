using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Rates.Commands.RateMovie;

public record RateMovieCommand(
    int MovieId,
    decimal Rating
) : IRequest<Result>;
