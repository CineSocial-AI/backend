using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Keywords.Queries.Search;

public class SearchKeywordsQueryHandler : IRequestHandler<SearchKeywordsQuery, Result<object>>
{
    private readonly IApplicationDbContext _context;

    public SearchKeywordsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<object>> Handle(SearchKeywordsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Keywords;

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(k => k.Name.Contains(request.SearchTerm));
            }

            var total = await query.CountAsync(cancellationToken);

            var keywords = await query
                .OrderBy(k => k.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var result = new
            {
                data = keywords,
                total,
                page = request.Page,
                pageSize = request.PageSize
            };

            return Result<object>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure($"Failed to search keywords: {ex.Message}");
        }
    }
}
