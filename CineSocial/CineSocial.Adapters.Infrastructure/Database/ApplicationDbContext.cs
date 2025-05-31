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

    // DbSets for domain entities
    // public DbSet<Movie> Movies { get; set; }
    // public DbSet<Review> Reviews { get; set; }
    // Şimdilik sadece User var

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all configurations from assembly
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configure Identity tables with custom names
        builder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.ProfileImageUrl).HasMaxLength(500);

            // PostgreSQL için GUID column type
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
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Handle audit properties
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

        // ✅ Dispatch domain events AFTER saving to database
        await DispatchDomainEventsAsync();

        return result;
    }

    private async Task DispatchDomainEventsAsync()
    {
        // 1. Find all entities with domain events
        var domainEntities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        // 2. Collect all domain events
        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        // 3. Clear domain events from entities (prevent re-processing)
        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        // 4. ✅ ACTUAL IMPLEMENTATION: Publish events using MediatR
        foreach (var domainEvent in domainEvents)
        {
            try
            {
                await _mediator.Publish(domainEvent);
            }
            catch (Exception ex)
            {
                // Log error but don't fail the transaction
                // You might want to inject ILogger here
                Console.WriteLine($"Error publishing domain event {domainEvent.GetType().Name}: {ex.Message}");
            }
        }
    }
}