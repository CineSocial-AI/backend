using CineSocial.Domain.Entities.Movie;
using CineSocial.Domain.Entities.Social;
using CineSocial.Domain.Entities.User;

namespace CineSocial.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // User entities
    IQueryable<AppUser> Users { get; }
    IQueryable<Image> Images { get; }
    IQueryable<Follow> Follows { get; }
    IQueryable<Block> Blocks { get; }

    // Movie entities
    IQueryable<MovieEntity> Movies { get; }
    IQueryable<Genre> Genres { get; }
    IQueryable<MovieGenre> MovieGenres { get; }
    IQueryable<Person> People { get; }
    IQueryable<MovieCast> MovieCasts { get; }
    IQueryable<MovieCrew> MovieCrews { get; }
    IQueryable<ProductionCompany> ProductionCompanies { get; }
    IQueryable<MovieProductionCompany> MovieProductionCompanies { get; }
    IQueryable<Country> Countries { get; }
    IQueryable<MovieCountry> MovieCountries { get; }
    IQueryable<Language> Languages { get; }
    IQueryable<MovieLanguage> MovieLanguages { get; }
    IQueryable<Collection> Collections { get; }
    IQueryable<MovieCollection> MovieCollections { get; }
    IQueryable<Keyword> Keywords { get; }
    IQueryable<MovieKeyword> MovieKeywords { get; }

    // Social entities
    IQueryable<Comment> Comments { get; }
    IQueryable<Reaction> Reactions { get; }
    IQueryable<Rate> Rates { get; }
    IQueryable<MovieList> MovieLists { get; }
    IQueryable<MovieListItem> MovieListItems { get; }
    IQueryable<MovieListFavorite> MovieListFavorites { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    void Add<T>(T entity) where T : class;
    void Remove<T>(T entity) where T : class;
    void RemoveRange<T>(IEnumerable<T> entities) where T : class;
}
