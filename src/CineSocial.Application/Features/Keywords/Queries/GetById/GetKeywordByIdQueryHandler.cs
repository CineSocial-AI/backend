using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Keywords.Queries.GetById;

public class GetKeywordByIdQueryHandler : IRequestHandler<GetKeywordByIdQuery, Result<Keyword>>
{
    private readonly IApplicationDbContext _context;

    public GetKeywordByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Keyword>> Handle(GetKeywordByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var keyword = await _context.Keywords
                .FirstOrDefaultAsync(k => k.Id == request.Id, cancellationToken);

            if (keyword == null)
                return Result<Keyword>.Failure("Keyword not found");

            return Result<Keyword>.Success(keyword);
        }
        catch (Exception ex)
        {
            return Result<Keyword>.Failure($"Failed to retrieve keyword: {ex.Message}");
        }
    }
}
