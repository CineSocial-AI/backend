using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Rates.Queries.GetMovieRates;

public class GetMovieRatesQueryHandler : IRequestHandler<GetMovieRatesQuery, Result<object>>
{
    private readonly IRepository<Rate> _rateRepository;

    public GetMovieRatesQueryHandler(IRepository<Rate> rateRepository)
    {
        _rateRepository = rateRepository;
    }

    public async Task<Result<object>> Handle(GetMovieRatesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _rateRepository.GetQueryable()
                .Where(r => r.MovieId == request.MovieId);

            var total = await query.CountAsync(cancellationToken);

            var rates = await query
                .Include(r => r.User)
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
            return Result<object>.Failure($"Failed to retrieve movie rates: {ex.Message}");
        }
    }
}
