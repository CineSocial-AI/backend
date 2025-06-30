using CineSocial.Core.Domain.Common;
using CineSocial.Core.Domain.Entities;

namespace CineSocial.Core.Application.Ports.Repositories;

/// <summary>
/// Unit of Work interface for managing repository transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the repository for the specified entity type
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
    /// <returns>Repository instance</returns>
    IRepository<T> Repository<T>() where T : BaseEntity;

    /// <summary>
    /// Gets the movie repository
    /// </summary>
    IMovieRepository Movies { get; }

    /// <summary>
    /// Gets the user repository (special case since User inherits from IdentityUser)
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Saves all changes to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of affected records</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transaction instance</returns>
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Transaction interface for database transactions
/// </summary>
public interface ITransaction : IDisposable
{
    /// <summary>
    /// Commits the transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}