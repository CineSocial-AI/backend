using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;

namespace CineSocial.Application.Features.People.Queries.GetById;

public record GetPersonByIdQuery(int Id) : IRequest<Result<Person>>;
