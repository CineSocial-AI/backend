using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Lists.Commands.RemoveMovieFromList;

public record RemoveMovieFromListCommand(int ListId, int MovieId) : IRequest<Result<bool>>;
