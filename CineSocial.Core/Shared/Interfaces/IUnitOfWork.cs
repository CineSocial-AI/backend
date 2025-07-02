using CineSocial.Domain.Entities;

namespace CineSocial.Core.Shared.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IRepository<Movie> Movies { get; }
    IRepository<Person> Persons { get; }
    IRepository<Genre> Genres { get; }
    IRepository<MovieCast> MovieCasts { get; }
    IRepository<MovieCrew> MovieCrews { get; }
    IRepository<MovieGenre> MovieGenres { get; }
    IRepository<Review> Reviews { get; }
    IRepository<Rating> Ratings { get; }
    IRepository<Favorite> Favorites { get; }
    IRepository<Comment> Comments { get; }
    IRepository<Reaction> Reactions { get; }
    IRepository<MovieList> MovieLists { get; }
    IRepository<MovieListItem> MovieListItems { get; }
    IRepository<ListFavorite> ListFavorites { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}