using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Languages.Queries.GetById;

public class GetLanguageByIdQueryHandler : IRequestHandler<GetLanguageByIdQuery, Result<Language>>
{
    private readonly IApplicationDbContext _context;

    public GetLanguageByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Language>> Handle(GetLanguageByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var language = await _context.Languages
                .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

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
