using CineSocial.Core.Domain.Entities;

namespace CineSocial.Core.Application.Ports.Repositories;

/// <summary>
/// Repository interface for User entities
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by id
    /// </summary>
    /// <param name="id">User id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by email
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by username
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users with pagination
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of users</returns>
    Task<IEnumerable<User>> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches users by name or username
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of matching users</returns>
    Task<IEnumerable<User>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users who follow a specific user
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of followers</returns>
    Task<IEnumerable<User>> GetFollowersAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users that a specific user follows
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of users being followed</returns>
    Task<IEnumerable<User>> GetFollowingAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user
    /// </summary>
    /// <param name="user">User to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user
    /// </summary>
    /// <param name="user">User to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user
    /// </summary>
    /// <param name="user">User to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);
}