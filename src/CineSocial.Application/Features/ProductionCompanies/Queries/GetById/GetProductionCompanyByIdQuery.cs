using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;

namespace CineSocial.Application.Features.ProductionCompanies.Queries.GetById;

public record GetProductionCompanyByIdQuery(int Id) : IRequest<Result<ProductionCompany>>;
