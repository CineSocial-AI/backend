using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Countries.Queries.GetAll;

public class GetAllCountriesQueryHandler : IRequestHandler<GetAllCountriesQuery, Result<List<Country>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllCountriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<Country>>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var countries = await _context.Countries
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);

            return Result<List<Country>>.Success(countries);
        }
        catch (Exception ex)
        {
            return Result<List<Country>>.Failure($"Failed to retrieve countries: {ex.Message}");
        }
    }
}
