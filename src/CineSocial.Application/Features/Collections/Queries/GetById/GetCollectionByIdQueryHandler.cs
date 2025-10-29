using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Collections.Queries.GetById;

public class GetCollectionByIdQueryHandler : IRequestHandler<GetCollectionByIdQuery, Result<Collection>>
{
    private readonly IApplicationDbContext _context;

    public GetCollectionByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Collection>> Handle(GetCollectionByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var collection = await _context.Collections
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (collection == null)
                return Result<Collection>.Failure("Collection not found");

            return Result<Collection>.Success(collection);
        }
        catch (Exception ex)
        {
            return Result<Collection>.Failure($"Failed to retrieve collection: {ex.Message}");
        }
    }
}
