using CineSocial.Domain.Entities;
using CineSocial.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Infrastructure.Data;

public class CineSocialDbContext : DbContext
{
    public CineSocialDbContext(DbContextOptions<CineSocialDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<MovieCast> MovieCasts { get; set; }
    public DbSet<MovieCrew> MovieCrews { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<MovieList> MovieLists { get; set; }
    public DbSet<MovieListItem> MovieListItems { get; set; }
    public DbSet<ListFavorite> ListFavorites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new MovieConfiguration());
        modelBuilder.ApplyConfiguration(new RatingConfiguration());

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CineSocialDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var entity = (BaseEntity)entityEntry.Entity;

            if (entityEntry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}