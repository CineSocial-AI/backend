using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;

namespace CineSocial.Application.Features.Lists.Queries.GetMovieListById;

public record GetMovieListByIdQuery(int ListId) : IRequest<Result<MovieList>>;
