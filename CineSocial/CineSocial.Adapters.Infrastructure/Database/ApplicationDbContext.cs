using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MediatR;
using CineSocial.Core.Domain.Entities;
using CineSocial.Core.Domain.Common;
using System.Reflection;

namespace CineSocial.Adapters.Infrastructure.Database;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    private readonly IMediator _mediator;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Watchlist> Watchlists { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<ReviewLike> ReviewLikes { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<MovieCast> MovieCasts { get; set; }
    public DbSet<MovieCrew> MovieCrews { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.ProfileImageUrl).HasMaxLength(500);
            entity.Property(e => e.Id).HasColumnType("uuid");
        });

        builder.Entity<IdentityRole<Guid>>(entity =>
        {
            entity.ToTable("Roles");
            entity.Property(e => e.Id).HasColumnType("uuid");
        });

        builder.Entity<IdentityUserRole<Guid>>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.Property(e => e.RoleId).HasColumnType("uuid");
        });

        builder.Entity<IdentityUserClaim<Guid>>(entity =>
        {
            entity.ToTable("UserClaims");
            entity.Property(e => e.UserId).HasColumnType("uuid");
        });

        builder.Entity<IdentityUserLogin<Guid>>(entity =>
        {
            entity.ToTable("UserLogins");
            entity.Property(e => e.UserId).HasColumnType("uuid");
        });

        builder.Entity<IdentityRoleClaim<Guid>>(entity =>
        {
            entity.ToTable("RoleClaims");
            entity.Property(e => e.RoleId).HasColumnType("uuid");
        });

        builder.Entity<IdentityUserToken<Guid>>(entity =>
        {
            entity.ToTable("UserTokens");
            entity.Property(e => e.UserId).HasColumnType("uuid");
        });

        builder.Entity<Movie>(entity =>
        {
            entity.ToTable("Movies");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.OriginalTitle).HasMaxLength(200);
            entity.Property(e => e.Overview).HasMaxLength(2000);
            entity.Property(e => e.PosterPath).HasMaxLength(500);
            entity.Property(e => e.BackdropPath).HasMaxLength(500);
            entity.Property(e => e.Language).HasMaxLength(10);
            entity.Property(e => e.Homepage).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Tagline).HasMaxLength(500);
            entity.Property(e => e.ImdbId).HasMaxLength(20);
            entity.Property(e => e.VoteAverage).HasColumnType("decimal(3,1)");
            entity.Property(e => e.Popularity).HasColumnType("decimal(8,3)");
            entity.HasIndex(e => e.TmdbId).IsUnique();
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.ReleaseDate);
        });

        builder.Entity<Genre>(entity =>
        {
            entity.ToTable("Genres");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.TmdbId).IsUnique();
        });

        builder.Entity<Review>(entity =>
        {
            entity.ToTable("Reviews");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.Property(e => e.MovieId).HasColumnType("uuid");
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Content).HasMaxLength(5000).IsRequired();
            entity.Property(e => e.Rating).HasColumnType("decimal(3,1)").IsRequired();
            entity.HasIndex(e => new { e.UserId, e.MovieId }).IsUnique();
            entity.HasIndex(e => e.MovieId);
            entity.HasIndex(e => e.CreatedAt);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Movie)
                .WithMany(m => m.Reviews)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Rating>(entity =>
        {
            entity.ToTable("Ratings");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.Property(e => e.MovieId).HasColumnType("uuid");
            entity.Property(e => e.Value).HasColumnType("decimal(3,1)").IsRequired();
            entity.HasIndex(e => new { e.UserId, e.MovieId }).IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Movie)
                .WithMany(m => m.Ratings)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Watchlist>(entity =>
        {
            entity.ToTable("Watchlists");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.Property(e => e.MovieId).HasColumnType("uuid");
            entity.HasIndex(e => new { e.UserId, e.MovieId }).IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Watchlists)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Movie)
                .WithMany(m => m.Watchlists)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comments");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.Property(e => e.ReviewId).HasColumnType("uuid");
            entity.Property(e => e.ParentCommentId).HasColumnType("uuid");
            entity.Property(e => e.Content).HasMaxLength(1000).IsRequired();
            entity.HasIndex(e => e.ReviewId);
            entity.HasIndex(e => e.ParentCommentId);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Review)
                .WithMany(r => r.Comments)
                .HasForeignKey(e => e.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(e => e.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ReviewLike>(entity =>
        {
            entity.ToTable("ReviewLikes");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.Property(e => e.ReviewId).HasColumnType("uuid");
            entity.HasIndex(e => new { e.UserId, e.ReviewId }).IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(u => u.ReviewLikes)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Review)
                .WithMany(r => r.ReviewLikes)
                .HasForeignKey(e => e.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CommentLike>(entity =>
        {
            entity.ToTable("CommentLikes");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.Property(e => e.CommentId).HasColumnType("uuid");
            entity.HasIndex(e => new { e.UserId, e.CommentId }).IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(u => u.CommentLikes)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Comment)
                .WithMany(c => c.CommentLikes)
                .HasForeignKey(e => e.CommentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<MovieGenre>(entity =>
        {
            entity.ToTable("MovieGenres");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.MovieId).HasColumnType("uuid");
            entity.Property(e => e.GenreId).HasColumnType("uuid");
            entity.HasIndex(e => new { e.MovieId, e.GenreId }).IsUnique();

            entity.HasOne(e => e.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Genre)
                .WithMany(g => g.MovieGenres)
                .HasForeignKey(e => e.GenreId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Person>(entity =>
        {
            entity.ToTable("Persons");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Biography).HasMaxLength(2000);
            entity.Property(e => e.PlaceOfBirth).HasMaxLength(200);
            entity.Property(e => e.ProfilePath).HasMaxLength(500);
            entity.Property(e => e.ImdbId).HasMaxLength(20);
            entity.Property(e => e.Popularity).HasColumnType("decimal(8,3)");
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.KnownForDepartment).HasMaxLength(50);
            entity.HasIndex(e => e.TmdbId).IsUnique();
            entity.HasIndex(e => e.Name);
        });

        builder.Entity<MovieCast>(entity =>
        {
            entity.ToTable("MovieCasts");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.MovieId).HasColumnType("uuid");
            entity.Property(e => e.PersonId).HasColumnType("uuid");
            entity.Property(e => e.Character).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => new { e.MovieId, e.PersonId, e.Character }).IsUnique();

            entity.HasOne(e => e.Movie)
                .WithMany(m => m.MovieCasts)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Person)
                .WithMany(p => p.MovieCasts)
                .HasForeignKey(e => e.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<MovieCrew>(entity =>
        {
            entity.ToTable("MovieCrews");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.MovieId).HasColumnType("uuid");
            entity.Property(e => e.PersonId).HasColumnType("uuid");
            entity.Property(e => e.Job).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Department).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => new { e.MovieId, e.PersonId, e.Job }).IsUnique();

            entity.HasOne(e => e.Movie)
                .WithMany(m => m.MovieCrews)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Person)
                .WithMany(p => p.MovieCrews)
                .HasForeignKey(e => e.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
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

        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchDomainEventsAsync();

        return result;
    }

    private async Task DispatchDomainEventsAsync()
    {
        var domainEntities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            try
            {
                await _mediator.Publish(domainEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing domain event {domainEvent.GetType().Name}: {ex.Message}");
            }
        }
    }
}