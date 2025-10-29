using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Keywords.Queries.Search;

public record SearchKeywordsQuery(string? SearchTerm, int Page = 1, int PageSize = 20) : IRequest<Result<object>>;
