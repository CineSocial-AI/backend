using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Lists.Commands.AddMovieToList;

public record AddMovieToListCommand(int ListId, int MovieId) : IRequest<Result<bool>>;
