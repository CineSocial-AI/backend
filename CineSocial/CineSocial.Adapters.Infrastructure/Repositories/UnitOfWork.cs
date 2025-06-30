using Microsoft.EntityFrameworkCore.Storage;
using CineSocial.Core.Application.Ports.Repositories;
using CineSocial.Core.Domain.Entities;
using CineSocial.Adapters.Infrastructure.Database;

namespace CineSocial.Adapters.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation for managing repositories and transactions
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repository instances
    private IMovieRepository? _movies;
    private IRepository<Genre>? _genres;
    private IRepository<Review>? _reviews;
    private IRepository<Rating>? _ratings;
    private IRepository<Watchlist>? _watchlists;
    private IRepository<Comment>? _comments;
    private IRepository<Group>? _groups;
    private IRepository<Post>? _posts;
    private IRepository<PostComment>? _postComments;
    private IRepository<User>? _users;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // Repository properties with lazy initialization
    public IMovieRepository Movies => _movies ??= new MovieRepository(_context);
    public IRepository<Genre> Genres => _genres ??= new Repository<Genre>(_context);
    public IRepository<Review> Reviews => _reviews ??= new Repository<Review>(_context);
    public IRepository<Rating> Ratings => _ratings ??= new Repository<Rating>(_context);
    public IRepository<Watchlist> Watchlists => _watchlists ??= new Repository<Watchlist>(_context);
    public IRepository<Comment> Comments => _comments ??= new Repository<Comment>(_context);
    public IRepository<Group> Groups => _groups ??= new Repository<Group>(_context);
    public IRepository<Post> Posts => _posts ??= new Repository<Post>(_context);
    public IRepository<PostComment> PostComments => _postComments ??= new Repository<PostComment>(_context);
    public IRepository<User> Users => _users ??= new Repository<User>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}