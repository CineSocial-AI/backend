using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Languages.Queries.GetByIso;

public class GetLanguageByIsoQueryHandler : IRequestHandler<GetLanguageByIsoQuery, Result<Language>>
{
    private readonly IApplicationDbContext _context;

    public GetLanguageByIsoQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Language>> Handle(GetLanguageByIsoQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var language = await _context.Languages
                .FirstOrDefaultAsync(l => l.Iso6391 == request.Iso.ToLower(), cancellationToken);

            if (language == null)
                return Result<Language>.Failure("Language not found");

            return Result<Language>.Success(language);
        }
        catch (Exception ex)
        {
            return Result<Language>.Failure($"Failed to retrieve language: {ex.Message}");
        }
    }
}
