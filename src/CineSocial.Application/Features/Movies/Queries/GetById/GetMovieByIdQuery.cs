using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Movies.Queries.GetById;

public record GetMovieByIdQuery(int Id) : IRequest<Result<MovieDetailDto>>;
