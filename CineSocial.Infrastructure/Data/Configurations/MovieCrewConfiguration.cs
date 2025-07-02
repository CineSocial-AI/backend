using CineSocial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineSocial.Infrastructure.Data.Configurations;

public class MovieCrewConfiguration : IEntityTypeConfiguration<MovieCrew>
{
    public void Configure(EntityTypeBuilder<MovieCrew> builder)
    {
        builder.HasKey(mc => mc.Id);

        builder.Property(mc => mc.Job)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(mc => mc.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(mc => mc.Movie)
            .WithMany(m => m.MovieCrews)
            .HasForeignKey(mc => mc.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mc => mc.Person)
            .WithMany(p => p.MovieCrews)
            .HasForeignKey(mc => mc.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}