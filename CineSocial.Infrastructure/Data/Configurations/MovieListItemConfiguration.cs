using CineSocial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineSocial.Infrastructure.Data.Configurations;

public class MovieListItemConfiguration : IEntityTypeConfiguration<MovieListItem>
{
    public void Configure(EntityTypeBuilder<MovieListItem> builder)
    {
        builder.HasKey(mli => mli.Id);

        builder.Property(mli => mli.Notes)
            .HasMaxLength(500);

        builder.HasIndex(mli => new { mli.MovieListId, mli.MovieId })
            .IsUnique();

        builder.HasOne(mli => mli.MovieList)
            .WithMany(ml => ml.MovieListItems)
            .HasForeignKey(mli => mli.MovieListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mli => mli.Movie)
            .WithMany(m => m.MovieListItems)
            .HasForeignKey(mli => mli.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}