using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Movies.Queries.GetTopRated;

public record GetTopRatedMoviesQuery(int Page = 1, int PageSize = 20, int MinVotes = 100) : IRequest<Result<object>>;
