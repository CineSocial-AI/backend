using Microsoft.EntityFrameworkCore;
using CineSocial.Core.Application.Ports.Repositories;
using CineSocial.Core.Domain.Entities;
using CineSocial.Adapters.Infrastructure.Database;

namespace CineSocial.Adapters.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for User entities
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.IsActive && 
                       (u.FirstName.Contains(searchTerm) || 
                        u.LastName.Contains(searchTerm) || 
                        u.UserName!.Contains(searchTerm)))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetFollowersAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Followings
            .Where(f => f.FollowingId == userId)
            .Select(f => f.Follower)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetFollowingAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Followings
            .Where(f => f.FollowerId == userId)
            .Select(f => f.FollowedUser)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(user);
        return Task.CompletedTask;
    }
}