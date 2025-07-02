using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CineSocial.Api.DTOs;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using CineSocial.Infrastructure.Data;
using CineSocial.Tests.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CineSocial.Tests.Integration.Infrastructure;

public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    protected readonly HttpClient Client;
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly IServiceScope Scope;
    protected readonly CineSocialDbContext DbContext;
    protected readonly IPasswordHasher PasswordHasher;
    private readonly string DatabaseName = $"TestDb_{Guid.NewGuid()}";

    protected readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<CineSocialDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing - use same database name for all contexts
                services.AddDbContext<CineSocialDbContext>(options =>
                {
                    options.UseInMemoryDatabase(DatabaseName);
                });

                // Ensure logging is configured properly for tests
                services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
            });
        });

        Client = Factory.CreateClient();
        Scope = Factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<CineSocialDbContext>();
        PasswordHasher = Scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        // Ensure database is created
        DbContext.Database.EnsureCreated();
    }

    protected async Task<User> CreateTestUserAsync(string email = "test@example.com", string username = "testuser", string password = "password123")
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = PasswordHasher.HashPassword(password),
            FirstName = "Test",
            LastName = "User",
            IsActive = true,
            IsEmailVerified = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();
        return user;
    }

    protected async Task<Movie> CreateTestMovieAsync(string title = "Test Movie")
    {
        var movie = TestHelper.CreateTestMovie(title);
        DbContext.Movies.Add(movie);
        await DbContext.SaveChangesAsync();
        return movie;
    }

    protected async Task<Genre> CreateTestGenreAsync(string name = "Action")
    {
        var genre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = $"{name} movies",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        DbContext.Genres.Add(genre);
        await DbContext.SaveChangesAsync();
        return genre;
    }

    protected async Task<AuthResponse> LoginUserAsync(string email = "test@example.com", string password = "password123")
    {
        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AuthResponse>(content, JsonOptions)!;
    }

    protected void SetAuthorizationHeader(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    protected void ClearAuthorizationHeader()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }

    protected async Task ClearDatabaseAsync()
    {
        // Clear all entities from the database in correct order (dependencies first)
        if (DbContext.MovieListItems.Any())
            DbContext.MovieListItems.RemoveRange(DbContext.MovieListItems);
        
        if (DbContext.MovieLists.Any())
            DbContext.MovieLists.RemoveRange(DbContext.MovieLists);
        
        if (DbContext.Reactions.Any())
            DbContext.Reactions.RemoveRange(DbContext.Reactions);
        
        if (DbContext.Comments.Any())
            DbContext.Comments.RemoveRange(DbContext.Comments);
        
        if (DbContext.Favorites.Any())
            DbContext.Favorites.RemoveRange(DbContext.Favorites);
        
        if (DbContext.Ratings.Any())
            DbContext.Ratings.RemoveRange(DbContext.Ratings);
        
        if (DbContext.Reviews.Any())
            DbContext.Reviews.RemoveRange(DbContext.Reviews);
        
        if (DbContext.MovieGenres.Any())
            DbContext.MovieGenres.RemoveRange(DbContext.MovieGenres);
        
        if (DbContext.MovieCrews.Any())
            DbContext.MovieCrews.RemoveRange(DbContext.MovieCrews);
        
        if (DbContext.MovieCasts.Any())
            DbContext.MovieCasts.RemoveRange(DbContext.MovieCasts);
        
        if (DbContext.Movies.Any())
            DbContext.Movies.RemoveRange(DbContext.Movies);
        
        if (DbContext.Genres.Any())
            DbContext.Genres.RemoveRange(DbContext.Genres);
        
        if (DbContext.Persons.Any())
            DbContext.Persons.RemoveRange(DbContext.Persons);
        
        if (DbContext.Users.Any())
            DbContext.Users.RemoveRange(DbContext.Users);

        if (DbContext.ChangeTracker.HasChanges())
        {
            await DbContext.SaveChangesAsync();
        }
    }

    protected void RefreshDbContext()
    {
        // Clear change tracker to ensure fresh data from database
        DbContext.ChangeTracker.Clear();
    }

    public void Dispose()
    {
        DbContext?.Dispose();
        Scope?.Dispose();
        Client?.Dispose();
    }
}