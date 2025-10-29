using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;

namespace CineSocial.Application.Features.Lists.Queries.GetPublicMovieLists;

public record GetPublicMovieListsQuery(int Page, int PageSize) : IRequest<Result<List<MovieList>>>;
