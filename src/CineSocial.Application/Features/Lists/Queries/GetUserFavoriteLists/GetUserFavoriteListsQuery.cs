using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;

namespace CineSocial.Application.Features.Lists.Queries.GetUserFavoriteLists;

public record GetUserFavoriteListsQuery : IRequest<Result<List<MovieList>>>;
