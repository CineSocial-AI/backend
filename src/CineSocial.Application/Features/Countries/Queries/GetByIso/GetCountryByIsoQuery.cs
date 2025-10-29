using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;

namespace CineSocial.Application.Features.Countries.Queries.GetByIso;

public record GetCountryByIsoQuery(string Iso) : IRequest<Result<Country>>;
