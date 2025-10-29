using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;

namespace CineSocial.Application.Features.Genres.Queries.GetAll;

public record GetAllGenresQuery : IRequest<Result<List<Genre>>>;
