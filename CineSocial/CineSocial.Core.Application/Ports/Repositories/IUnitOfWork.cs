using CineSocial.Core.Domain.Entities;

namespace CineSocial.Core.Application.Ports.Repositories;

/// <summary>
/// Unit of work interface for managing transactions across repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repository properties
    IMovieRepository Movies { get; }
    IRepository<Genre> Genres { get; }
    IRepository<Review> Reviews { get; }
    IRepository<Rating> Ratings { get; }
    IRepository<Watchlist> Watchlists { get; }
    IRepository<Comment> Comments { get; }
    IRepository<Group> Groups { get; }
    IRepository<Post> Posts { get; }
    IRepository<PostComment> PostComments { get; }
    IRepository<User> Users { get; }
    
    // Transaction management
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}