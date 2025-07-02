using CineSocial.Core.Shared.Interfaces;
using CineSocial.Infrastructure.Data;
using CineSocial.Infrastructure.Logging;
using CineSocial.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CineSocial.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddRepositories();
        services.AddServices();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? Environment.GetEnvironmentVariable("DATABASE_URL");

        var useInMemoryDatabase = bool.Parse(configuration["UseInMemoryDatabase"] ?? "false");

        services.AddScoped<DatabaseCommandInterceptor>();
        
        services.AddDbContext<CineSocialDbContext>((serviceProvider, options) =>
        {
            var interceptor = serviceProvider.GetRequiredService<DatabaseCommandInterceptor>();
            
            if (useInMemoryDatabase || string.IsNullOrEmpty(connectionString))
            {
                options.UseInMemoryDatabase("CineSocialInMemoryDb");
            }
            else
            {
                options.UseNpgsql(connectionString);
            }
            
            options.AddInterceptors(interceptor);
            options.EnableSensitiveDataLogging(false);
            options.EnableServiceProviderCaching();
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}