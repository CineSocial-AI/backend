using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using CineSocial.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CineSocial.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly CineSocialDbContext _context;
    private IDbContextTransaction? _transaction;

    private IUserRepository? _users;
    private IRepository<Movie>? _movies;
    private IRepository<Person>? _persons;
    private IRepository<Genre>? _genres;
    private IRepository<MovieCast>? _movieCasts;
    private IRepository<MovieCrew>? _movieCrews;
    private IRepository<MovieGenre>? _movieGenres;
    private IRepository<Review>? _reviews;
    private IRepository<Rating>? _ratings;
    private IRepository<Favorite>? _favorites;
    private IRepository<Comment>? _comments;
    private IRepository<Reaction>? _reactions;
    private IRepository<MovieList>? _movieLists;
    private IRepository<MovieListItem>? _movieListItems;
    private IRepository<ListFavorite>? _listFavorites;

    public UnitOfWork(CineSocialDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IRepository<Movie> Movies => _movies ??= new Repository<Movie>(_context);
    public IRepository<Person> Persons => _persons ??= new Repository<Person>(_context);
    public IRepository<Genre> Genres => _genres ??= new Repository<Genre>(_context);
    public IRepository<MovieCast> MovieCasts => _movieCasts ??= new Repository<MovieCast>(_context);
    public IRepository<MovieCrew> MovieCrews => _movieCrews ??= new Repository<MovieCrew>(_context);
    public IRepository<MovieGenre> MovieGenres => _movieGenres ??= new Repository<MovieGenre>(_context);
    public IRepository<Review> Reviews => _reviews ??= new Repository<Review>(_context);
    public IRepository<Rating> Ratings => _ratings ??= new Repository<Rating>(_context);
    public IRepository<Favorite> Favorites => _favorites ??= new Repository<Favorite>(_context);
    public IRepository<Comment> Comments => _comments ??= new Repository<Comment>(_context);
    public IRepository<Reaction> Reactions => _reactions ??= new Repository<Reaction>(_context);
    public IRepository<MovieList> MovieLists => _movieLists ??= new Repository<MovieList>(_context);
    public IRepository<MovieListItem> MovieListItems => _movieListItems ??= new Repository<MovieListItem>(_context);
    public IRepository<ListFavorite> ListFavorites => _listFavorites ??= new Repository<ListFavorite>(_context);

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