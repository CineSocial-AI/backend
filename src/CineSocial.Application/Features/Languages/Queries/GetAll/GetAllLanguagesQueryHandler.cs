using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Languages.Queries.GetAll;

public class GetAllLanguagesQueryHandler : IRequestHandler<GetAllLanguagesQuery, Result<List<Language>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllLanguagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<Language>>> Handle(GetAllLanguagesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var languages = await _context.Languages
                .OrderBy(l => l.Name)
                .ToListAsync(cancellationToken);

            return Result<List<Language>>.Success(languages);
        }
        catch (Exception ex)
        {
            return Result<List<Language>>.Failure($"Failed to retrieve languages: {ex.Message}");
        }
    }
}
