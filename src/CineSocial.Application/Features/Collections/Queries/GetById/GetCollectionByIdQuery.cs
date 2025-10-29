using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;

namespace CineSocial.Application.Features.Collections.Queries.GetById;

public record GetCollectionByIdQuery(int Id) : IRequest<Result<Collection>>;
