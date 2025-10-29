using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.People.Queries.Search;

public record SearchPeopleQuery(string? SearchTerm, int Page = 1, int PageSize = 20) : IRequest<Result<object>>;
