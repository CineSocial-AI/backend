using CineSocial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineSocial.Infrastructure.Data.Configurations;

public class MovieListConfiguration : IEntityTypeConfiguration<MovieList>
{
    public void Configure(EntityTypeBuilder<MovieList> builder)
    {
        builder.HasKey(ml => ml.Id);

        builder.Property(ml => ml.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ml => ml.Description)
            .HasMaxLength(1000);

        builder.HasOne(ml => ml.User)
            .WithMany(u => u.MovieLists)
            .HasForeignKey(ml => ml.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ml => ml.MovieListItems)
            .WithOne(mli => mli.MovieList)
            .HasForeignKey(mli => mli.MovieListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}