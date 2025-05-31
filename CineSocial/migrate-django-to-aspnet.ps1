# Update ApplicationDbContext with new entities
# Run this script from: D:\code\CineSocial\backend\CineSocial\

Write-Host "Updating ApplicationDbContext with new movie entities..." -ForegroundColor Green

$dbContextPath = "CineSocial.Adapters.Infrastructure\Database\ApplicationDbContext.cs"

if (Test-Path $dbContextPath) {
    # Read current content
    $dbContextContent = Get-Content $dbContextPath -Raw
    
    # New DbSets to add after the constructor
    $newDbSets = @'

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
'@

    # Entity configurations to add before the closing brace of OnModelCreating
    $newEntityConfigurations = @'

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
'@

    # Find the position to insert DbSets (after the constructor)
    $constructorEnd = $dbContextContent.IndexOf("}")
    if ($constructorEnd -gt 0) {
        # Find the first occurrence after the constructor
        $insertDbSetsPosition = $dbContextContent.IndexOf("protected override void OnModelCreating", $constructorEnd)
        if ($insertDbSetsPosition -gt 0) {
            $beforeDbSets = $dbContextContent.Substring(0, $insertDbSetsPosition)
            $afterDbSets = $dbContextContent.Substring($insertDbSetsPosition)
            $dbContextContent = $beforeDbSets + $newDbSets + "`n`n    " + $afterDbSets
        }
    }

    # Find the last closing brace in OnModelCreating method to add entity configurations
    $lastBracePosition = $dbContextContent.LastIndexOf("    }")
    if ($lastBracePosition -gt 0) {
        $beforeConfigs = $dbContextContent.Substring(0, $lastBracePosition)
        $afterConfigs = $dbContextContent.Substring($lastBracePosition)
        $dbContextContent = $beforeConfigs + $newEntityConfigurations + "`n" + $afterConfigs
    }

    # Write the updated content back
    Set-Content -Path $dbContextPath -Value $dbContextContent -Encoding UTF8

    Write-Host "ApplicationDbContext updated successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Added DbSets:" -ForegroundColor Yellow
    Write-Host "  - Movies, Genres, Reviews, Ratings" -ForegroundColor White
    Write-Host "  - Watchlists, Comments, ReviewLikes, CommentLikes" -ForegroundColor White
    Write-Host "  - MovieGenres, Persons, MovieCasts, MovieCrews" -ForegroundColor White
    Write-Host ""
    Write-Host "Added Entity Configurations:" -ForegroundColor Yellow
    Write-Host "  - All table configurations with constraints" -ForegroundColor White
    Write-Host "  - Foreign key relationships" -ForegroundColor White
    Write-Host "  - Indexes for performance" -ForegroundColor White
    Write-Host "  - Column types for PostgreSQL" -ForegroundColor White
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "  1. dotnet ef migrations add AddMovieEntities" -ForegroundColor White
    Write-Host "  2. dotnet ef database update" -ForegroundColor White
    Write-Host ""
    Write-Host "Database schema is ready for movies!" -ForegroundColor Green
} else {
    Write-Host "ApplicationDbContext.cs file not found!" -ForegroundColor Red
    Write-Host "Make sure you're running this from the correct directory." -ForegroundColor Red
}