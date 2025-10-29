using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Rates.Queries.GetUserRatings;

public class GetUserRatingsQueryHandler : IRequestHandler<GetUserRatingsQuery, Result<object>>
{
    private readonly IRepository<Rate> _rateRepository;

    public GetUserRatingsQueryHandler(IRepository<Rate> rateRepository)
    {
        _rateRepository = rateRepository;
    }

    public async Task<Result<object>> Handle(GetUserRatingsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _rateRepository.GetQueryable()
                .Where(r => r.UserId == request.UserId);

            var total = await query.CountAsync(cancellationToken);

            var rates = await query
                .OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var result = new
            {
                data = rates,
                total,
                page = request.Page,
                pageSize = request.PageSize
            };

            return Result<object>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure($"Failed to retrieve user ratings: {ex.Message}");
        }
    }
}
