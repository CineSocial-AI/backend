using CineSocial.Application.Common.Security;
using CineSocial.Domain.Entities.User;
using CineSocial.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Infrastructure.Data.Seed;

public static class AppUserSeed
{
    public static void SeedUsers(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<AppUser>().HasData(
            new AppUser
            {
                Id = 1,
                Username = "user",
                Email = "user@cinesocial.com",
                PasswordHash = PasswordHasher.HashPassword("User123!"),
                Role = UserRole.User,
                Bio = "Regular user account",
                IsActive = true,
                IsDeleted = false,
                CreatedAt = seedDate,
                CreatedBy = "System"
            },
            new AppUser
            {
                Id = 2,
                Username = "admin",
                Email = "admin@cinesocial.com",
                PasswordHash = PasswordHasher.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                Bio = "Administrator account",
                IsActive = true,
                IsDeleted = false,
                CreatedAt = seedDate,
                CreatedBy = "System"
            },
            new AppUser
            {
                Id = 3,
                Username = "superuser",
                Email = "superuser@cinesocial.com",
                PasswordHash = PasswordHasher.HashPassword("SuperUser123!"),
                Role = UserRole.SuperUser,
                Bio = "Super User account with full privileges",
                IsActive = true,
                IsDeleted = false,
                CreatedAt = seedDate,
                CreatedBy = "System"
            }
        );
    }
}
