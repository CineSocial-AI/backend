using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Rates.Commands.RemoveRate;

public record RemoveRateCommand(
    int MovieId
) : IRequest<Result>;
