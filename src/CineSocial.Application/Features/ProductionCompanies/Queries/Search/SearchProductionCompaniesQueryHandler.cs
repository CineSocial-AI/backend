using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.ProductionCompanies.Queries.Search;

public class SearchProductionCompaniesQueryHandler : IRequestHandler<SearchProductionCompaniesQuery, Result<object>>
{
    private readonly IApplicationDbContext _context;

    public SearchProductionCompaniesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<object>> Handle(SearchProductionCompaniesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.ProductionCompanies;

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(pc => pc.Name.Contains(request.SearchTerm));
            }

            var total = await query.CountAsync(cancellationToken);

            var companies = await query
                .OrderBy(pc => pc.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var result = new
            {
                data = companies,
                total,
                page = request.Page,
                pageSize = request.PageSize
            };

            return Result<object>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure($"Failed to search production companies: {ex.Message}");
        }
    }
}
