using System.Security.Claims;
using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.User;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class UserQueries
{
    /// <summary>
    /// Get current authenticated user
    /// </summary>
    [Authorize]
    public async Task<AppUser?> GetMe(
        [Service] IRepository<AppUser> repository,
        [Service] IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? user?.FindFirst("sub")?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        return await repository.GetQueryable()
            .Include(u => u.Followers)
            .Include(u => u.Following)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    /// <summary>
    /// Get all users (for browsing/search)
    /// </summary>
    [UseProjection]
    public IQueryable<AppUser> GetUsers(
        [Service] IRepository<AppUser> repository)
    {
        return repository.GetQueryable()
            .Where(u => !u.IsDeleted)
            .OrderBy(u => u.Username);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    public async Task<AppUser?> GetUser(
        int id,
        [Service] IRepository<AppUser> repository,
        CancellationToken cancellationToken)
    {
        return await repository.GetQueryable()
            .Include(u => u.Followers)
            .Include(u => u.Following)
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Get user by username
    /// </summary>
    public async Task<AppUser?> GetUserByUsername(
        string username,
        [Service] IRepository<AppUser> repository,
        CancellationToken cancellationToken)
    {
        return await repository.GetQueryable()
            .Include(u => u.Followers)
            .Include(u => u.Following)
            .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Search users by username
    /// </summary>
    [UseProjection]
    public IQueryable<AppUser> SearchUsers(
        string searchTerm,
        [Service] IRepository<AppUser> repository)
    {
        return repository.GetQueryable()
            .Where(u => !u.IsDeleted && u.Username.Contains(searchTerm))
            .OrderBy(u => u.Username)
            .Take(20);
    }
}
