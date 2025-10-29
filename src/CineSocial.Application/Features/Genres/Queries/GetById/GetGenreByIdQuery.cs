using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;

namespace CineSocial.Application.Features.Genres.Queries.GetById;

public record GetGenreByIdQuery(int Id) : IRequest<Result<Genre>>;
