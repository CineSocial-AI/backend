using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Lists.Commands.FavoriteMovieList;

public record FavoriteMovieListCommand(int ListId) : IRequest<Result<bool>>;
