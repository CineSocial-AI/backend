using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Collections.Queries.GetMovies;

public record GetCollectionMoviesQuery(int CollectionId) : IRequest<Result<object>>;
