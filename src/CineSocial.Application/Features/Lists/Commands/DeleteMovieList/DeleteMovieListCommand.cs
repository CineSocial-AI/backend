using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Lists.Commands.DeleteMovieList;

public record DeleteMovieListCommand(int ListId) : IRequest<Result<bool>>;
