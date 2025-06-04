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

    // Movie-related DbSets (existing)
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

    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostMedia> PostMedias { get; set; }
    public DbSet<PostComment> PostComments { get; set; }
    public DbSet<PostReaction> PostReactions { get; set; }
    public DbSet<CommentReaction> CommentReactions { get; set; }
    public DbSet<GroupBan> GroupBans { get; set; }
    public DbSet<UserBlock> UserBlocks { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<Following> Followings { get; set; }

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

        // Movie-related entities configuration (existing)
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

        // Reddit-like platform entities configuration (new)
        builder.Entity<Group>(entity =>
        {
            entity.ToTable("Groups");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Rules).HasMaxLength(5000);
            entity.Property(e => e.IconUrl).HasMaxLength(500);
            entity.Property(e => e.BannerUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedById).HasColumnType("uuid");
            entity.HasIndex(e => e.Name).IsUnique();

            entity.HasOne(e => e.CreatedBy)
                .WithMany(u => u.CreatedGroups)
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<GroupMember>(entity =>
        {
            entity.ToTable("GroupMembers");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.GroupId).HasColumnType("uuid");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.HasIndex(e => new { e.GroupId, e.UserId }).IsUnique();

            entity.HasOne(e => e.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.GroupMemberships)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Post>(entity =>
        {
            entity.ToTable("Posts");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.AuthorId).HasColumnType("uuid");
            entity.Property(e => e.GroupId).HasColumnType("uuid");
            entity.Property(e => e.Title).HasMaxLength(300).IsRequired();
            entity.Property(e => e.Content).HasMaxLength(10000);
            entity.Property(e => e.Url).HasMaxLength(1000);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.GroupId);
            entity.HasIndex(e => e.AuthorId);

            entity.HasOne(e => e.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Group)
                .WithMany(g => g.Posts)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PostMedia>(entity =>
        {
            entity.ToTable("PostMedias");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.PostId).HasColumnType("uuid");
            entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Url).HasMaxLength(1000).IsRequired();
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(1000);
            entity.Property(e => e.MimeType).HasMaxLength(100);

            entity.HasOne(e => e.Post)
                .WithMany(p => p.Media)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PostComment>(entity =>
        {
            entity.ToTable("PostComments");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.PostId).HasColumnType("uuid");
            entity.Property(e => e.AuthorId).HasColumnType("uuid");
            entity.Property(e => e.ParentCommentId).HasColumnType("uuid");
            entity.Property(e => e.Content).HasMaxLength(2000).IsRequired();
            entity.HasIndex(e => e.PostId);
            entity.HasIndex(e => e.AuthorId);
            entity.HasIndex(e => e.ParentCommentId);

            entity.HasOne(e => e.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Author)
                .WithMany(u => u.PostComments)
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(e => e.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PostReaction>(entity =>
        {
            entity.ToTable("PostReactions");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.PostId).HasColumnType("uuid");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.HasIndex(e => new { e.PostId, e.UserId }).IsUnique();

            entity.HasOne(e => e.Post)
                .WithMany(p => p.Reactions)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.PostReactions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CommentReaction>(entity =>
        {
            entity.ToTable("CommentReactions");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.CommentId).HasColumnType("uuid");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.HasIndex(e => new { e.CommentId, e.UserId }).IsUnique();

            entity.HasOne(e => e.Comment)
                .WithMany(c => c.Reactions)
                .HasForeignKey(e => e.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.CommentReactions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<GroupBan>(entity =>
        {
            entity.ToTable("GroupBans");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.GroupId).HasColumnType("uuid");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.Property(e => e.BannedById).HasColumnType("uuid");
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.HasIndex(e => new { e.GroupId, e.UserId });

            entity.HasOne(e => e.Group)
                .WithMany(g => g.Bans)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.GroupBans)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.BannedBy)
                .WithMany(u => u.IssuedBans)
                .HasForeignKey(e => e.BannedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<UserBlock>(entity =>
        {
            entity.ToTable("UserBlocks");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.BlockerId).HasColumnType("uuid");
            entity.Property(e => e.BlockedId).HasColumnType("uuid");
            entity.HasIndex(e => new { e.BlockerId, e.BlockedId }).IsUnique();

            entity.HasOne(e => e.Blocker)
                .WithMany(u => u.BlockedUsers)
                .HasForeignKey(e => e.BlockerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Blocked)
                .WithMany(u => u.BlockedByUsers)
                .HasForeignKey(e => e.BlockedId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Report>(entity =>
        {
            entity.ToTable("Reports");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.ReporterId).HasColumnType("uuid");
            entity.Property(e => e.TargetId).HasColumnType("uuid");
            entity.Property(e => e.ReviewedById).HasColumnType("uuid");
            entity.Property(e => e.Details).HasMaxLength(1000);
            entity.Property(e => e.ReviewNotes).HasMaxLength(1000);

            entity.HasOne(e => e.Reporter)
                .WithMany(u => u.Reports)
                .HasForeignKey(e => e.ReporterId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ReviewedBy)
                .WithMany(u => u.ReviewedReports)
                .HasForeignKey(e => e.ReviewedById)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<PostTag>(entity =>
        {
            entity.ToTable("PostTags");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.PostId).HasColumnType("uuid");
            entity.Property(e => e.Tag).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Tag);

            entity.HasOne(e => e.Post)
                .WithMany(p => p.Tags)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Following>(entity =>
        {
            entity.ToTable("Followings");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.FollowerId).HasColumnType("uuid");
            entity.Property(e => e.FollowingId).HasColumnType("uuid");
            entity.HasIndex(e => new { e.FollowerId, e.FollowingId }).IsUnique();

            entity.HasOne(e => e.Follower)
                .WithMany(u => u.Followings)
                .HasForeignKey(e => e.FollowerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.FollowedUser)
                .WithMany(u => u.Followers)
                .HasForeignKey(e => e.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);
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


    public async Task SeedDataAsync(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {

        // 1. KULLANICILAR OLUŞTUR
        var users = new List<User>();
        var userPasswords = new Dictionary<string, string>();

        var user1 = new User
        {
            Id = Guid.NewGuid(),
            UserName = "cinemalovers",
            Email = "cinema@example.com",
            FirstName = "Ahmet",
            LastName = "Yılmaz",
            Bio = "Film tutkunu. Özellikle aksiyon ve sci-fi filmleri seviyorum.",
            EmailConfirmed = true,
            IsActive = true,
            DateOfBirth = new DateTime(1990, 5, 15, 0, 0, 0, DateTimeKind.Utc),
            CreatedAt = DateTime.UtcNow.AddDays(-100)
        };
        userPasswords[user1.Email] = "Cinema123!";
        users.Add(user1);

        var user2 = new User
        {
            Id = Guid.NewGuid(),
            UserName = "moviecritique",
            Email = "critic@example.com",
            FirstName = "Fatma",
            LastName = "Demir",
            Bio = "Profesyonel film eleştirmeni. Her türden filme açığım.",
            EmailConfirmed = true,
            IsActive = true,
            DateOfBirth = new DateTime(1985, 12, 3, 0, 0, 0, DateTimeKind.Utc),
            CreatedAt = DateTime.UtcNow.AddDays(-85)
        };
        userPasswords[user2.Email] = "Critic123!";
        users.Add(user2);

        var user3 = new User
        {
            Id = Guid.NewGuid(),
            UserName = "dramaqueenn",
            Email = "drama@example.com",
            FirstName = "Elif",
            LastName = "Kaya",
            Bio = "Drama ve romantik film uzmanı. Duygu yüklü hikayeler beni büyülüyor.",
            EmailConfirmed = true,
            IsActive = true,
            DateOfBirth = new DateTime(1993, 8, 20, 0, 0, 0, DateTimeKind.Utc),
            CreatedAt = DateTime.UtcNow.AddDays(-70)
        };
        userPasswords[user3.Email] = "Drama123!";
        users.Add(user3);

        var user4 = new User
        {
            Id = Guid.NewGuid(),
            UserName = "horrormaster",
            Email = "horror@example.com",
            FirstName = "Mehmet",
            LastName = "Özkan",
            Bio = "Korku filmi koleksiyoncusu. Gece yarısı film izlemek en büyük hobim.",
            EmailConfirmed = true,
            IsActive = true,
            DateOfBirth = new DateTime(1988, 10, 31, 0, 0, 0, DateTimeKind.Utc),
            CreatedAt = DateTime.UtcNow.AddDays(-90)
        };
        userPasswords[user4.Email] = "Horror123!";
        users.Add(user4);

        var user5 = new User
        {
            Id = Guid.NewGuid(),
            UserName = "comedyfan",
            Email = "comedy@example.com",
            FirstName = "Zeynep",
            LastName = "Arslan",
            Bio = "Komedi severim. Hayatta mizah çok önemli, filmler de öyle!",
            EmailConfirmed = true,
            IsActive = true,
            DateOfBirth = new DateTime(1995, 3, 14, 0, 0, 0, DateTimeKind.Utc),
            CreatedAt = DateTime.UtcNow.AddDays(-60)
        };
        userPasswords[user5.Email] = "Comedy123!";
        users.Add(user5);

        // Kullanıcıları veritabanına ekle
        foreach (var user in users)
        {
            var result = await userManager.CreateAsync(user, userPasswords[user.Email]);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
            }
        }

        // 2. TÜRLER OLUŞTUR
        var genres = new List<Genre>
    {
        new Genre { Id = Guid.NewGuid(), Name = "Aksiyon", Description = "Hareketli ve gerilim dolu filmler", TmdbId = 28, CreatedAt = DateTime.UtcNow.AddDays(-120) },
        new Genre { Id = Guid.NewGuid(), Name = "Komedi", Description = "Güldürmeyi amaçlayan filmler", TmdbId = 35, CreatedAt = DateTime.UtcNow.AddDays(-120) },
        new Genre { Id = Guid.NewGuid(), Name = "Drama", Description = "Duygusal ve karaktere odaklı filmler", TmdbId = 18, CreatedAt = DateTime.UtcNow.AddDays(-120) },
        new Genre { Id = Guid.NewGuid(), Name = "Korku", Description = "Korkutmayı amaçlayan filmler", TmdbId = 27, CreatedAt = DateTime.UtcNow.AddDays(-120) },
        new Genre { Id = Guid.NewGuid(), Name = "Bilim Kurgu", Description = "Gelecek ve teknoloji odaklı filmler", TmdbId = 878, CreatedAt = DateTime.UtcNow.AddDays(-120) },
        new Genre { Id = Guid.NewGuid(), Name = "Romantik", Description = "Aşk hikayelerini anlatan filmler", TmdbId = 10749, CreatedAt = DateTime.UtcNow.AddDays(-120) },
        new Genre { Id = Guid.NewGuid(), Name = "Gerilim", Description = "Merak ve heyecan yaratan filmler", TmdbId = 53, CreatedAt = DateTime.UtcNow.AddDays(-120) }
    };

        await Genres.AddRangeAsync(genres);
        await SaveChangesAsync();

        // 3. FİLMLER OLUŞTUR
        var movies = new List<Movie>
    {
        new Movie
        {
            Id = Guid.NewGuid(),
            Title = "The Dark Knight",
            OriginalTitle = "The Dark Knight",
            Overview = "Batman, Joker'in Gotham şehrini kaosa sürüklemesini engellemeye çalışır.",
            ReleaseDate = new DateTime(2008, 7, 18, 0, 0, 0, DateTimeKind.Utc),
            Runtime = 152,
            VoteAverage = 9.0m,
            VoteCount = 25000,
            Popularity = 95.5m,
            Language = "en",
            Status = "Released",
            Budget = 185000000,
            Revenue = 1004558444,
            TmdbId = 155,
            ImdbId = "tt0468569",
            PosterPath = "/qJ2tW6WMUDux911r6m7haRef0WH.jpg",
            BackdropPath = "/hqkIcbrOHL86UncnHIsHVcVmzue.jpg",
            CreatedAt = DateTime.UtcNow.AddDays(-110)
        },
        new Movie
        {
            Id = Guid.NewGuid(),
            Title = "Inception",
            OriginalTitle = "Inception",
            Overview = "Rüya dünyasına girip, insanların bilinçaltlarından sır çalmaya odaklanan hırsız Dom Cobb.",
            ReleaseDate = new DateTime(2010, 7, 16, 0, 0, 0, DateTimeKind.Utc),
            Runtime = 148,
            VoteAverage = 8.8m,
            VoteCount = 31000,
            Popularity = 88.2m,
            Language = "en",
            Status = "Released",
            Budget = 160000000,
            Revenue = 829895144,
            TmdbId = 27205,
            ImdbId = "tt1375666",
            PosterPath = "/9gk7adHYeDvHkCSEqAvQNLV5Uge.jpg",
            BackdropPath = "/s3TBrRGB1iav7gFOCNx3H31MoES.jpg",
            CreatedAt = DateTime.UtcNow.AddDays(-105)
        },
        new Movie
        {
            Id = Guid.NewGuid(),
            Title = "The Shawshank Redemption",
            OriginalTitle = "The Shawshank Redemption",
            Overview = "Haksız yere hapse atılan Andy Dufresne'nin hapishane yaşamı ve arkadaşlığı.",
            ReleaseDate = new DateTime(1994, 9, 23, 0, 0, 0, DateTimeKind.Utc),
            Runtime = 142,
            VoteAverage = 9.3m,
            VoteCount = 24000,
            Popularity = 92.1m,
            Language = "en",
            Status = "Released",
            Budget = 25000000,
            Revenue = 16000000,
            TmdbId = 278,
            ImdbId = "tt0111161",
            PosterPath = "/q6y0Go1tsGEsmtFryDOJo3dEmqu.jpg",
            BackdropPath = "/kXfqcdQKsToO0OUXHcrrNCHDBzO.jpg",
            CreatedAt = DateTime.UtcNow.AddDays(-100)
        },
        new Movie
        {
            Id = Guid.NewGuid(),
            Title = "Pulp Fiction",
            OriginalTitle = "Pulp Fiction",
            Overview = "Los Angeles'ta geçen, birbirine karışan suç hikayelerini anlatan film.",
            ReleaseDate = new DateTime(1994, 10, 14, 0, 0, 0, DateTimeKind.Utc),
            Runtime = 154,
            VoteAverage = 8.9m,
            VoteCount = 22000,
            Popularity = 87.3m,
            Language = "en",
            Status = "Released",
            Budget = 8000000,
            Revenue = 214200000,
            TmdbId = 680,
            ImdbId = "tt0110912",
            PosterPath = "/d5iIlFn5s0ImszYzBPb8JPIfbXD.jpg",
            BackdropPath = "/4cDFJr4HnXN5AdPw4AKrmLlMWdO.jpg",
            CreatedAt = DateTime.UtcNow.AddDays(-95)
        },
        new Movie
        {
            Id = Guid.NewGuid(),
            Title = "The Conjuring",
            OriginalTitle = "The Conjuring",
            Overview = "Korku uzmanları Ed ve Lorraine Warren'ın gerçek hayat deneyimlerinden uyarlanan korku filmi.",
            ReleaseDate = new DateTime(2013, 7, 19, 0, 0, 0, DateTimeKind.Utc),
            Runtime = 112,
            VoteAverage = 7.5m,
            VoteCount = 15000,
            Popularity = 78.9m,
            Language = "en",
            Status = "Released",
            Budget = 20000000,
            Revenue = 319494638,
            TmdbId = 138843,
            ImdbId = "tt1457767",
            PosterPath = "/wVYREutTvI2tmxr6ujrHT704wGF.jpg",
            BackdropPath = "/rrwDzCwE5q1Y9zm7VyGG33cAW9x.jpg",
            CreatedAt = DateTime.UtcNow.AddDays(-90)
        },
        new Movie
        {
            Id = Guid.NewGuid(),
            Title = "Titanic",
            OriginalTitle = "Titanic",
            Overview = "1912'de batan Titanic gemisinde geçen trajik aşk hikayesi.",
            ReleaseDate = new DateTime(1997, 12, 19, 0, 0, 0, DateTimeKind.Utc),
            Runtime = 194,
            VoteAverage = 7.9m,
            VoteCount = 20000,
            Popularity = 89.7m,
            Language = "en",
            Status = "Released",
            Budget = 200000000,
            Revenue = 2187463944,
            TmdbId = 597,
            ImdbId = "tt0120338",
            PosterPath = "/9xjZS2rlVxm8SFx8kPC3aIGCOYQ.jpg",
            BackdropPath = "/kHXEpyfl6zqn8a6YuozZUujufXf.jpg",
            CreatedAt = DateTime.UtcNow.AddDays(-85)
        }
    };

        await Movies.AddRangeAsync(movies);
        await SaveChangesAsync();

        // 4. FİLM-TÜR İLİŞKİLERİ
        var movieGenres = new List<MovieGenre>
    {
        new MovieGenre { Id = Guid.NewGuid(), MovieId = movies[0].Id, GenreId = genres[0].Id, CreatedAt = DateTime.UtcNow.AddDays(-110) },
        new MovieGenre { Id = Guid.NewGuid(), MovieId = movies[0].Id, GenreId = genres[6].Id, CreatedAt = DateTime.UtcNow.AddDays(-110) },
        new MovieGenre { Id = Guid.NewGuid(), MovieId = movies[1].Id, GenreId = genres[0].Id, CreatedAt = DateTime.UtcNow.AddDays(-105) },
        new MovieGenre { Id = Guid.NewGuid(), MovieId = movies[1].Id, GenreId = genres[4].Id, CreatedAt = DateTime.UtcNow.AddDays(-105) },
        new MovieGenre { Id = Guid.NewGuid(), MovieId = movies[2].Id, GenreId = genres[2].Id, CreatedAt = DateTime.UtcNow.AddDays(-100) },
        new MovieGenre { Id = Guid.NewGuid(), MovieId = movies[3].Id, GenreId = genres[2].Id, CreatedAt = DateTime.UtcNow.AddDays(-95) },
        new MovieGenre { Id = Guid.NewGuid(), MovieId = movies[3].Id, GenreId = genres[6].Id, CreatedAt = DateTime.UtcNow.AddDays(-95) },
        new MovieGenre { Id = Guid.NewGuid(), MovieId = movies[4].Id, GenreId = genres[3].Id, CreatedAt = DateTime.UtcNow.AddDays(-90) },
        new MovieGenre { Id = Guid.NewGuid(), MovieId = movies[5].Id, GenreId = genres[2].Id, CreatedAt = DateTime.UtcNow.AddDays(-85) },
        new MovieGenre { Id = Guid.NewGuid(), MovieId = movies[5].Id, GenreId = genres[5].Id, CreatedAt = DateTime.UtcNow.AddDays(-85) }
    };

        await MovieGenres.AddRangeAsync(movieGenres);
        await SaveChangesAsync();

        // 5. GRUPLAR OLUŞTUR
        var groups = new List<Group>
    {
        new Group
        {
            Id = Guid.NewGuid(),
            Name = "AksiyonSeverler",
            Description = "Aksiyon filmi tutkunlarının buluşma noktası. En iyi aksiyon filmleri hakkında konuşuyoruz.",
            Rules = "1. Spoiler vermek yasak\n2. Saygılı olun\n3. Film önerilerini kategorize edin",
            IsPrivate = false,
            RequireApproval = false,
            IsNsfw = false,
            CreatedById = users[0].Id,
            MemberCount = 3,
            PostCount = 3,
            CreatedAt = DateTime.UtcNow.AddDays(-80)
        },
        new Group
        {
            Id = Guid.NewGuid(),
            Name = "KorkuSinemaKlubu",
            Description = "Korku filmi meraklıları için özel kulüp. En korkutucu filmlerden klasiklere kadar her şey burada.",
            Rules = "1. Korku seviyesini belirtin\n2. Zayıf kalpliler için uyarı verin\n3. Tarihsel korku filmleri tercih edilir",
            IsPrivate = false,
            RequireApproval = true,
            IsNsfw = true,
            CreatedById = users[3].Id,
            MemberCount = 2,
            PostCount = 2,
            CreatedAt = DateTime.UtcNow.AddDays(-75)
        },
        new Group
        {
            Id = Guid.NewGuid(),
            Name = "FilmElestirmenleri",
            Description = "Profesyonel ve amatör film eleştirmenlerinin objektif yorumlar yaptığı alan.",
            Rules = "1. Objektif eleştiri yapın\n2. Teknik detaylara odaklanın\n3. Saygısız yorumlar silinir",
            IsPrivate = false,
            RequireApproval = false,
            IsNsfw = false,
            CreatedById = users[1].Id,
            MemberCount = 4,
            PostCount = 3,
            CreatedAt = DateTime.UtcNow.AddDays(-70)
        }
    };

        await Groups.AddRangeAsync(groups);
        await SaveChangesAsync();

        // 6. WATCHLIST (İzleme Listeleri)
        var watchlists = new List<Watchlist>
    {
        new Watchlist { Id = Guid.NewGuid(), UserId = users[0].Id, MovieId = movies[1].Id, IsWatched = false, CreatedAt = DateTime.UtcNow.AddDays(-50) },
        new Watchlist { Id = Guid.NewGuid(), UserId = users[0].Id, MovieId = movies[4].Id, IsWatched = true, WatchedDate = DateTime.UtcNow.AddDays(-30), CreatedAt = DateTime.UtcNow.AddDays(-45) },
        new Watchlist { Id = Guid.NewGuid(), UserId = users[1].Id, MovieId = movies[0].Id, IsWatched = true, WatchedDate = DateTime.UtcNow.AddDays(-45), CreatedAt = DateTime.UtcNow.AddDays(-55) },
        new Watchlist { Id = Guid.NewGuid(), UserId = users[1].Id, MovieId = movies[3].Id, IsWatched = false, CreatedAt = DateTime.UtcNow.AddDays(-40) },
        new Watchlist { Id = Guid.NewGuid(), UserId = users[2].Id, MovieId = movies[1].Id, IsWatched = true, WatchedDate = DateTime.UtcNow.AddDays(-40), CreatedAt = DateTime.UtcNow.AddDays(-48) },
        new Watchlist { Id = Guid.NewGuid(), UserId = users[3].Id, MovieId = movies[2].Id, IsWatched = false, CreatedAt = DateTime.UtcNow.AddDays(-42) },
        new Watchlist { Id = Guid.NewGuid(), UserId = users[4].Id, MovieId = movies[0].Id, IsWatched = false, CreatedAt = DateTime.UtcNow.AddDays(-38) },
        new Watchlist { Id = Guid.NewGuid(), UserId = users[4].Id, MovieId = movies[5].Id, IsWatched = true, WatchedDate = DateTime.UtcNow.AddDays(-20), CreatedAt = DateTime.UtcNow.AddDays(-35) }
    };

        await Watchlists.AddRangeAsync(watchlists);
        await SaveChangesAsync();

        // 7. RATINGS (Puanlamalar)
        var ratings = new List<Rating>
    {
        new Rating { Id = Guid.NewGuid(), UserId = users[0].Id, MovieId = movies[0].Id, Value = 9.5m, CreatedAt = DateTime.UtcNow.AddDays(-45) },
        new Rating { Id = Guid.NewGuid(), UserId = users[1].Id, MovieId = movies[1].Id, Value = 9.0m, CreatedAt = DateTime.UtcNow.AddDays(-40) },
        new Rating { Id = Guid.NewGuid(), UserId = users[2].Id, MovieId = movies[2].Id, Value = 9.8m, CreatedAt = DateTime.UtcNow.AddDays(-35) },
        new Rating { Id = Guid.NewGuid(), UserId = users[3].Id, MovieId = movies[4].Id, Value = 8.5m, CreatedAt = DateTime.UtcNow.AddDays(-30) },
        new Rating { Id = Guid.NewGuid(), UserId = users[4].Id, MovieId = movies[3].Id, Value = 8.8m, CreatedAt = DateTime.UtcNow.AddDays(-25) },
        new Rating { Id = Guid.NewGuid(), UserId = users[2].Id, MovieId = movies[5].Id, Value = 8.2m, CreatedAt = DateTime.UtcNow.AddDays(-20) }
    };

        await Ratings.AddRangeAsync(ratings);
        await SaveChangesAsync();

        // 8. FİLM İNCELEMELERİ
        var reviews = new List<Review>
    {
        new Review
        {
            Id = Guid.NewGuid(),
            UserId = users[0].Id,
            MovieId = movies[0].Id,
            Title = "Süper Kahraman Sinemasının Zirvesi",
            Content = "The Dark Knight sadece bir süper kahraman filmi değil, aynı zamanda mükemmel bir gerilim filmi. Heath Ledger'ın Joker karakteri sinema tarihine geçti. Nolan'ın gerçekçi yaklaşımı ve felsefi derinliği filmi başyapıt yapıyor.",
            Rating = 9.5m,
            LikesCount = 25,
            DislikesCount = 2,
            IsSpoiler = false,
            CreatedAt = DateTime.UtcNow.AddDays(-45)
        },
        new Review
        {
            Id = Guid.NewGuid(),
            UserId = users[1].Id,
            MovieId = movies[1].Id,
            Title = "Rüya ve Gerçeklik Arasında Bir Yolculuk",
            Content = "Christopher Nolan bir kez daha zihinleri karıştırmayı başarıyor. Inception'ın rüya katmanları ve zamansal kompleksitesi film boyunca izleyiciyi merakta tutuyor. Leonardo DiCaprio'nun performansı mükemmel.",
            Rating = 9.0m,
            LikesCount = 32,
            DislikesCount = 1,
            IsSpoiler = true,
            CreatedAt = DateTime.UtcNow.AddDays(-40)
        },
        new Review
        {
            Id = Guid.NewGuid(),
            UserId = users[2].Id,
            MovieId = movies[2].Id,
            Title = "Umudun ve Dostluğun Gücü",
            Content = "The Shawshank Redemption insanlık duygularını bu kadar güzel işleyen nadir filmlerden. Tim Robbins ve Morgan Freeman'ın kimyası mükemmel. Her izleyişte yeni detaylar keşfedilen bir başyapıt.",
            Rating = 9.8m,
            LikesCount = 41,
            DislikesCount = 0,
            IsSpoiler = false,
            CreatedAt = DateTime.UtcNow.AddDays(-35)
        },
        new Review
        {
            Id = Guid.NewGuid(),
            UserId = users[3].Id,
            MovieId = movies[4].Id,
            Title = "Modern Korku Sinemasının Şaheseri",
            Content = "James Wan korku sinemasını nasıl yapılacağını gösteriyor. The Conjuring ucuz korkutma hileleri yerine atmosfer yaratmaya odaklanıyor. Gerçek olaylardan ilham alması filmi daha da korkutucu yapıyor.",
            Rating = 8.5m,
            LikesCount = 18,
            DislikesCount = 1,
            IsSpoiler = false,
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        }
    };

        await Reviews.AddRangeAsync(reviews);
        await SaveChangesAsync();

        // 9. İNCELEME BEĞENİLERİ
        var reviewLikes = new List<ReviewLike>
    {
        new ReviewLike { Id = Guid.NewGuid(), UserId = users[1].Id, ReviewId = reviews[0].Id, IsLike = true, CreatedAt = DateTime.UtcNow.AddDays(-44) },
        new ReviewLike { Id = Guid.NewGuid(), UserId = users[2].Id, ReviewId = reviews[0].Id, IsLike = true, CreatedAt = DateTime.UtcNow.AddDays(-43) },
        new ReviewLike { Id = Guid.NewGuid(), UserId = users[3].Id, ReviewId = reviews[0].Id, IsLike = true, CreatedAt = DateTime.UtcNow.AddDays(-42) },
        new ReviewLike { Id = Guid.NewGuid(), UserId = users[0].Id, ReviewId = reviews[1].Id, IsLike = true, CreatedAt = DateTime.UtcNow.AddDays(-39) },
        new ReviewLike { Id = Guid.NewGuid(), UserId = users[2].Id, ReviewId = reviews[1].Id, IsLike = true, CreatedAt = DateTime.UtcNow.AddDays(-38) },
        new ReviewLike { Id = Guid.NewGuid(), UserId = users[0].Id, ReviewId = reviews[2].Id, IsLike = true, CreatedAt = DateTime.UtcNow.AddDays(-34) },
        new ReviewLike { Id = Guid.NewGuid(), UserId = users[1].Id, ReviewId = reviews[2].Id, IsLike = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
        new ReviewLike { Id = Guid.NewGuid(), UserId = users[0].Id, ReviewId = reviews[3].Id, IsLike = true, CreatedAt = DateTime.UtcNow.AddDays(-29) }
    };

        await ReviewLikes.AddRangeAsync(reviewLikes);
        await SaveChangesAsync();

        // 10. İNCELEME YORUMLARI
        var reviewComments = new List<Comment>
    {
        new Comment
        {
            Id = Guid.NewGuid(),
            UserId = users[1].Id,
            ReviewId = reviews[0].Id,
            Content = "Heath Ledger'ın performansı konusunda kesinlikle katılıyorum. Oscar'dan sonra daha da anlamlı oldu.",
            LikeCount = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-43)
        },
        new Comment
        {
            Id = Guid.NewGuid(),
            UserId = users[3].Id,
            ReviewId = reviews[0].Id,
            Content = "Hans Zimmer'ın müziklerini de eklemeni güzel olmuş. Bu filmin her unsuru mükemmel.",
            LikeCount = 3,
            CreatedAt = DateTime.UtcNow.AddDays(-42)
        },
        new Comment
        {
            Id = Guid.NewGuid(),
            UserId = users[0].Id,
            ReviewId = reviews[1].Id,
            Content = "Rüya katmanları konusunda çok detaylı analiz yapmışsın. Son sahne teorin çok mantıklı.",
            LikeCount = 7,
            CreatedAt = DateTime.UtcNow.AddDays(-38)
        }
    };

        await Comments.AddRangeAsync(reviewComments);
        await SaveChangesAsync();

        // 11. TAKİP İLİŞKİLERİ
        var followings = new List<Following>
    {
        new Following { Id = Guid.NewGuid(), FollowerId = users[0].Id, FollowingId = users[1].Id, CreatedAt = DateTime.UtcNow.AddDays(-80) },
        new Following { Id = Guid.NewGuid(), FollowerId = users[0].Id, FollowingId = users[2].Id, CreatedAt = DateTime.UtcNow.AddDays(-75) },
        new Following { Id = Guid.NewGuid(), FollowerId = users[1].Id, FollowingId = users[0].Id, CreatedAt = DateTime.UtcNow.AddDays(-70) },
        new Following { Id = Guid.NewGuid(), FollowerId = users[1].Id, FollowingId = users[3].Id, CreatedAt = DateTime.UtcNow.AddDays(-65) },
        new Following { Id = Guid.NewGuid(), FollowerId = users[2].Id, FollowingId = users[1].Id, CreatedAt = DateTime.UtcNow.AddDays(-60) },
        new Following { Id = Guid.NewGuid(), FollowerId = users[3].Id, FollowingId = users[4].Id, CreatedAt = DateTime.UtcNow.AddDays(-55) },
        new Following { Id = Guid.NewGuid(), FollowerId = users[4].Id, FollowingId = users[0].Id, CreatedAt = DateTime.UtcNow.AddDays(-50) },
        new Following { Id = Guid.NewGuid(), FollowerId = users[4].Id, FollowingId = users[1].Id, CreatedAt = DateTime.UtcNow.AddDays(-45) }
    };

        await Followings.AddRangeAsync(followings);
        await SaveChangesAsync();

        Console.WriteLine("✅ Seed data başarıyla oluşturuldu!");
        Console.WriteLine($"👥 {users.Count} kullanıcı");
        Console.WriteLine($"🎬 {movies.Count} film");
        Console.WriteLine($"🏷️ {genres.Count} tür");
        Console.WriteLine($"👨‍👩‍👧‍👦 {groups.Count} grup");
        Console.WriteLine($"⭐ {reviews.Count} film incelemesi");
        Console.WriteLine($"💬 {reviewComments.Count} inceleme yorumu");
        Console.WriteLine($"📋 {watchlists.Count} izleme listesi öğesi");
        Console.WriteLine($"⭐ {ratings.Count} film puanlaması");
        Console.WriteLine($"👥 {followings.Count} takip ilişkisi");
    }
}
    