using CineSocial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineSocial.Infrastructure.Data.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.OriginalTitle)
            .HasMaxLength(500);

        builder.Property(m => m.Overview)
            .HasMaxLength(2000);

        builder.Property(m => m.PosterPath)
            .HasMaxLength(500);

        builder.Property(m => m.BackdropPath)
            .HasMaxLength(500);

        builder.Property(m => m.Language)
            .HasMaxLength(10);

        builder.Property(m => m.Homepage)
            .HasMaxLength(500);

        builder.Property(m => m.Status)
            .HasMaxLength(50);

        builder.Property(m => m.Tagline)
            .HasMaxLength(500);

        builder.Property(m => m.ImdbId)
            .HasMaxLength(20);

        builder.Property(m => m.VoteAverage)
            .HasColumnType("decimal(3,1)");

        builder.Property(m => m.Popularity)
            .HasColumnType("decimal(10,3)");

        builder.HasIndex(m => m.TmdbId)
            .IsUnique()
            .HasFilter("[TmdbId] IS NOT NULL");

        builder.HasIndex(m => m.ImdbId)
            .IsUnique()
            .HasFilter("[ImdbId] IS NOT NULL");

        builder.HasMany(m => m.MovieCasts)
            .WithOne(mc => mc.Movie)
            .HasForeignKey(mc => mc.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.MovieCrews)
            .WithOne(mc => mc.Movie)
            .HasForeignKey(mc => mc.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.MovieGenres)
            .WithOne(mg => mg.Movie)
            .HasForeignKey(mg => mg.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Reviews)
            .WithOne(r => r.Movie)
            .HasForeignKey(r => r.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Ratings)
            .WithOne(r => r.Movie)
            .HasForeignKey(r => r.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Favorites)
            .WithOne(f => f.Movie)
            .HasForeignKey(f => f.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.MovieListItems)
            .WithOne(mli => mli.Movie)
            .HasForeignKey(mli => mli.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}