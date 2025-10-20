using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Rates.Commands.RemoveRate;

public class RemoveRateCommandHandler : IRequestHandler<RemoveRateCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public RemoveRateCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(RemoveRateCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = 1;

        var rate = await _context.Rates
            .FirstOrDefaultAsync(r => r.MovieId == request.MovieId && r.UserId == currentUserId, cancellationToken);

        if (rate == null)
        {
            return Result.Failure("Rating not found");
        }

        _context.Remove(rate);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Rating removed successfully");
    }
}
