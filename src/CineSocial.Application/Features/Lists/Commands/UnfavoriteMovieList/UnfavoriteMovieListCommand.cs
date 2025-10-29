using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Lists.Commands.UnfavoriteMovieList;

public record UnfavoriteMovieListCommand(int ListId) : IRequest<Result<bool>>;
