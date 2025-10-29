using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Lists.Commands.ReorderMovieInList;

public record ReorderMovieInListCommand(int ListId, int MovieId, int NewOrder) : IRequest<Result<bool>>;
