using CineSocial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineSocial.Infrastructure.Data.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Biography)
            .HasMaxLength(2000);

        builder.Property(p => p.PlaceOfBirth)
            .HasMaxLength(200);

        builder.Property(p => p.ProfilePath)
            .HasMaxLength(500);

        builder.Property(p => p.ImdbId)
            .HasMaxLength(20);

        builder.Property(p => p.Gender)
            .HasMaxLength(20);

        builder.Property(p => p.KnownForDepartment)
            .HasMaxLength(100);

        builder.Property(p => p.Popularity)
            .HasColumnType("decimal(10,3)");

        builder.HasIndex(p => p.TmdbId)
            .IsUnique()
            .HasFilter("[TmdbId] IS NOT NULL");

        builder.HasIndex(p => p.ImdbId)
            .IsUnique()
            .HasFilter("[ImdbId] IS NOT NULL");

        builder.HasMany(p => p.MovieCasts)
            .WithOne(mc => mc.Person)
            .HasForeignKey(mc => mc.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.MovieCrews)
            .WithOne(mc => mc.Person)
            .HasForeignKey(mc => mc.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}