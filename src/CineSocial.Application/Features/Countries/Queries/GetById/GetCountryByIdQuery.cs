using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;

namespace CineSocial.Application.Features.Countries.Queries.GetById;

public record GetCountryByIdQuery(int Id) : IRequest<Result<Country>>;
