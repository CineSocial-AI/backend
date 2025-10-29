using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Features.Users.Queries.GetAll;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.Users;

public class GetUsersUseCase
{
    private readonly IApplicationDbContext _context;

    public GetUsersUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserSummaryDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.IsActive)
            .OrderBy(u => u.Username)
            .Select(u => new UserSummaryDto(
                u.Id,
                u.Username,
                u.Email,
                u.Bio))
            .ToListAsync(cancellationToken);
    }
}
