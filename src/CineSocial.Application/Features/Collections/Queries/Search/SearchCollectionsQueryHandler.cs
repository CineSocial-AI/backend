using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Collections.Queries.Search;

public class SearchCollectionsQueryHandler : IRequestHandler<SearchCollectionsQuery, Result<object>>
{
    private readonly IApplicationDbContext _context;

    public SearchCollectionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<object>> Handle(SearchCollectionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Collections;

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(c => c.Name.Contains(request.SearchTerm));
            }

            var total = await query.CountAsync(cancellationToken);

            var collections = await query
                .OrderBy(c => c.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var result = new
            {
                data = collections,
                total,
                page = request.Page,
                pageSize = request.PageSize
            };

            return Result<object>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure($"Failed to search collections: {ex.Message}");
        }
    }
}
