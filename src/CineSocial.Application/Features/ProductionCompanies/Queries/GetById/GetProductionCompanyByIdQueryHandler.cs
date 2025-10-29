using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.ProductionCompanies.Queries.GetById;

public class GetProductionCompanyByIdQueryHandler : IRequestHandler<GetProductionCompanyByIdQuery, Result<ProductionCompany>>
{
    private readonly IApplicationDbContext _context;

    public GetProductionCompanyByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProductionCompany>> Handle(GetProductionCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var company = await _context.ProductionCompanies
                .FirstOrDefaultAsync(pc => pc.Id == request.Id, cancellationToken);

            if (company == null)
                return Result<ProductionCompany>.Failure("Production company not found");

            return Result<ProductionCompany>.Success(company);
        }
        catch (Exception ex)
        {
            return Result<ProductionCompany>.Failure($"Failed to retrieve production company: {ex.Message}");
        }
    }
}
