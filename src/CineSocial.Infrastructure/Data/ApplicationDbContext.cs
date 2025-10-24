using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Common;
using CineSocial.Domain.Entities.User;
using CineSocial.Domain.Entities.Movie;
using CineSocial.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // User entities
    public DbSet<AppUser> Users { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Block> Blocks { get; set; }

    // Movie entities
    public DbSet<MovieEntity> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<MovieCast> MovieCasts { get; set; }
    public DbSet<MovieCrew> MovieCrews { get; set; }
    public DbSet<ProductionCompany> ProductionCompanies { get; set; }
    public DbSet<MovieProductionCompany> MovieProductionCompanies { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<MovieCountry> MovieCountries { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<MovieLanguage> MovieLanguages { get; set; }
    public DbSet<Collection> Collections { get; set; }
    public DbSet<MovieCollection> MovieCollections { get; set; }
    public DbSet<Keyword> Keywords { get; set; }
    public DbSet<MovieKeyword> MovieKeywords { get; set; }
    public DbSet<MovieVideo> MovieVideos { get; set; }
    public DbSet<MovieImage> MovieImages { get; set; }

    // Social entities
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<Rate> Rates { get; set; }
    public DbSet<MovieList> MovieLists { get; set; }
    public DbSet<MovieListItem> MovieListItems { get; set; }
    public DbSet<MovieListFavorite> MovieListFavorites { get; set; }

    IQueryable<AppUser> IApplicationDbContext.Users => Users;
    IQueryable<Image> IApplicationDbContext.Images => Images;
    IQueryable<Follow> IApplicationDbContext.Follows => Follows;
    IQueryable<Block> IApplicationDbContext.Blocks => Blocks;
    IQueryable<Comment> IApplicationDbContext.Comments => Comments;
    IQueryable<Reaction> IApplicationDbContext.Reactions => Reactions;
    IQueryable<Rate> IApplicationDbContext.Rates => Rates;
    IQueryable<MovieList> IApplicationDbContext.MovieLists => MovieLists;
    IQueryable<MovieListItem> IApplicationDbContext.MovieListItems => MovieListItems;
    IQueryable<MovieListFavorite> IApplicationDbContext.MovieListFavorites => MovieListFavorites;

    void IApplicationDbContext.Add<T>(T entity) => Set<T>().Add(entity);
    void IApplicationDbContext.Remove<T>(T entity) => Set<T>().Remove(entity);
    void IApplicationDbContext.RemoveRange<T>(IEnumerable<T> entities) => Set<T>().RemoveRange(entities);

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Seed data
        Seed.AppUserSeed.SeedUsers(modelBuilder);
    }
}
