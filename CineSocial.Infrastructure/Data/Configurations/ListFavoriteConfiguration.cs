using CineSocial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineSocial.Infrastructure.Data.Configurations;

public class ListFavoriteConfiguration : IEntityTypeConfiguration<ListFavorite>
{
    public void Configure(EntityTypeBuilder<ListFavorite> builder)
    {
        builder.HasKey(lf => lf.Id);

        builder.HasIndex(lf => new { lf.UserId, lf.MovieListId })
            .IsUnique();

        builder.HasOne(lf => lf.User)
            .WithMany(u => u.ListFavorites)
            .HasForeignKey(lf => lf.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(lf => lf.MovieList)
            .WithMany(ml => ml.ListFavorites)
            .HasForeignKey(lf => lf.MovieListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}