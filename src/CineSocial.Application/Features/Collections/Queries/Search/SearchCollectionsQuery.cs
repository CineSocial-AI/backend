using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Collections.Queries.Search;

public record SearchCollectionsQuery(string? SearchTerm, int Page = 1, int PageSize = 20) : IRequest<Result<object>>;
