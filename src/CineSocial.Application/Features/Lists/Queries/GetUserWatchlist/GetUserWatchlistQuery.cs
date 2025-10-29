using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;

namespace CineSocial.Application.Features.Lists.Queries.GetUserWatchlist;

public record GetUserWatchlistQuery : IRequest<Result<MovieList>>;
