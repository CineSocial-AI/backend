using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;

namespace CineSocial.Application.Features.Lists.Queries.GetUserMovieLists;

public record GetUserMovieListsQuery(int UserId) : IRequest<Result<List<MovieList>>>;
