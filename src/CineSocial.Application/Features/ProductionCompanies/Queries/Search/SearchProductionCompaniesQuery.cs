using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.ProductionCompanies.Queries.Search;

public record SearchProductionCompaniesQuery(string? SearchTerm, int Page = 1, int PageSize = 20) : IRequest<Result<object>>;
