using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Countries.Queries.GetById;

public class GetCountryByIdQueryHandler : IRequestHandler<GetCountryByIdQuery, Result<Country>>
{
    private readonly IApplicationDbContext _context;

    public GetCountryByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Country>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var country = await _context.Countries
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (country == null)
                return Result<Country>.Failure("Country not found");

            return Result<Country>.Success(country);
        }
        catch (Exception ex)
        {
            return Result<Country>.Failure($"Failed to retrieve country: {ex.Message}");
        }
    }
}
