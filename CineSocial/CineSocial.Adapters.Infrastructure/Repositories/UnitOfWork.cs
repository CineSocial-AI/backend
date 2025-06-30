using Microsoft.EntityFrameworkCore.Storage;
using CineSocial.Core.Application.Ports.Repositories;
using CineSocial.Core.Domain.Common;
using CineSocial.Core.Domain.Entities;
using CineSocial.Adapters.Infrastructure.Database;

namespace CineSocial.Adapters.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation for managing repositories and transactions
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;

    // Specific repository instances
    private IMovieRepository? _movies;
    private IUserRepository? _users;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the repository for the specified entity type
    /// </summary>
    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new Repository<T>(_context);
        }
        return (IRepository<T>)_repositories[type];
    }

    // Specific repository properties
    public IMovieRepository Movies => _movies ??= new MovieRepository(_context);
    public IUserRepository Users => _users ??= new UserRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return new Transaction(_transaction);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

/// <summary>
/// Transaction wrapper implementation
/// </summary>
public class Transaction : ITransaction
{
    private readonly IDbContextTransaction _transaction;

    public Transaction(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }
}