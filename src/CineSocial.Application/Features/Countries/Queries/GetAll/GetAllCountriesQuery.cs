using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;

namespace CineSocial.Application.Features.Countries.Queries.GetAll;

public record GetAllCountriesQuery : IRequest<Result<List<Country>>>;
