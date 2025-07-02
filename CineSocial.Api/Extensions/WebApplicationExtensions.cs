using CineSocial.Infrastructure.Data;
using CineSocial.Infrastructure.Data.Seeds;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CineSocial.Api.Extensions;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<CineSocialDbContext>>();

        try
        {
            var context = services.GetRequiredService<CineSocialDbContext>();
            
            // Check if using in-memory database
            var isInMemory = context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
            
            if (isInMemory)
            {
                // For in-memory database, just ensure it's created
                logger.LogInformation("Using in-memory database, ensuring database is created...");
                await context.Database.EnsureCreatedAsync();
            }
            else
            {
                // For real databases, run migrations
                if (context.Database.GetPendingMigrations().Any())
                {
                    logger.LogInformation("Applying pending migrations...");
                    await context.Database.MigrateAsync();
                }
            }
            
            // Seed data
            logger.LogInformation("Starting data seeding...");
            await DataSeeder.SeedDataAsync(services);
            logger.LogInformation("Data seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }

        return app;
    }

    public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<CineSocialDbContext>>();

        try
        {
            var context = services.GetRequiredService<CineSocialDbContext>();
            
            // Run migrations
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }

        return app;
    }
}