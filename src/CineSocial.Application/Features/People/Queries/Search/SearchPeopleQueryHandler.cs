using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.People.Queries.Search;

public class SearchPeopleQueryHandler : IRequestHandler<SearchPeopleQuery, Result<object>>
{
    private readonly IRepository<Person> _personRepository;

    public SearchPeopleQueryHandler(IRepository<Person> personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<Result<object>> Handle(SearchPeopleQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _personRepository.GetQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(request.SearchTerm));
            }

            var total = await query.CountAsync(cancellationToken);

            var people = await query
                .OrderBy(p => p.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var result = new
            {
                data = people,
                total,
                page = request.Page,
                pageSize = request.PageSize
            };

            return Result<object>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure($"Failed to search people: {ex.Message}");
        }
    }
}
