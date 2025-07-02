using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CineSocial.Api.DTOs;
using CineSocial.Domain.Entities;
using CineSocial.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CineSocial.Tests.Integration.Controllers;

public class FavoritesControllerIntegrationTests : IntegrationTestBase
{
    public FavoritesControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetUserFavorites_WithFavorites_ShouldReturnFavoritesList()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie1 = await CreateTestMovieAsync("Favorite Movie 1");
        var movie2 = await CreateTestMovieAsync("Favorite Movie 2");
        
        await CreateTestFavoriteAsync(user.Id, movie1.Id);
        await CreateTestFavoriteAsync(user.Id, movie2.Id);
        
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.GetAsync("/api/favorites");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        var favorites = JsonSerializer.Deserialize<List<FavoriteDto>>(
            jsonDoc.RootElement.GetProperty("data").GetRawText(),
            JsonOptions);

        favorites.Should().NotBeNull();
        favorites!.Should().HaveCount(2);
        favorites.Should().Contain(f => f.MovieTitle == "Favorite Movie 1");
        favorites.Should().Contain(f => f.MovieTitle == "Favorite Movie 2");
    }

    [Fact]
    public async Task GetUserFavorites_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        await ClearDatabaseAsync();
        ClearAuthorizationHeader();

        // Act
        var response = await Client.GetAsync("/api/favorites");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserFavorites_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Create multiple favorites
        for (int i = 1; i <= 15; i++)
        {
            var movie = await CreateTestMovieAsync($"Movie {i}");
            await CreateTestFavoriteAsync(user.Id, movie.Id);
        }

        // Act
        var response = await Client.GetAsync("/api/favorites?page=2&pageSize=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);

        var favorites = JsonSerializer.Deserialize<List<FavoriteDto>>(
            jsonDoc.RootElement.GetProperty("data").GetRawText(),
            JsonOptions);

        var pagination = jsonDoc.RootElement.GetProperty("pagination");

        favorites.Should().HaveCount(5);
        pagination.GetProperty("currentPage").GetInt32().Should().Be(2);
        pagination.GetProperty("pageSize").GetInt32().Should().Be(5);
        pagination.GetProperty("totalCount").GetInt32().Should().Be(15);
    }

    [Fact]
    public async Task GetUserFavorites_WithSorting_ShouldReturnSortedResults()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var movieA = await CreateTestMovieAsync("A Movie");
        var movieB = await CreateTestMovieAsync("B Movie");
        var movieC = await CreateTestMovieAsync("C Movie");

        await CreateTestFavoriteAsync(user.Id, movieC.Id);
        await CreateTestFavoriteAsync(user.Id, movieA.Id);
        await CreateTestFavoriteAsync(user.Id, movieB.Id);

        // Act
        var response = await Client.GetAsync("/api/favorites?sortBy=movie_title&sortOrder=asc");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        var favorites = JsonSerializer.Deserialize<List<FavoriteDto>>(
            jsonDoc.RootElement.GetProperty("data").GetRawText(),
            JsonOptions);

        favorites.Should().HaveCount(3);
        favorites.First().MovieTitle.Should().Be("A Movie");
        favorites.Last().MovieTitle.Should().Be("C Movie");
    }

    [Fact]
    public async Task GetUserFavorites_EmptyFavorites_ShouldReturnEmptyList()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestUserAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.GetAsync("/api/favorites");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        var favorites = JsonSerializer.Deserialize<List<FavoriteDto>>(
            jsonDoc.RootElement.GetProperty("data").GetRawText(),
            JsonOptions);

        favorites.Should().NotBeNull();
        favorites!.Should().BeEmpty();
    }

    [Fact]
    public async Task AddToFavorites_WithValidMovie_ShouldAddToFavorites()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync("Awesome Movie");
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var addRequest = new AddToFavoritesRequest
        {
            MovieId = movie.Id
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/favorites", addRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        var favoriteDto = JsonSerializer.Deserialize<FavoriteDto>(content, JsonOptions);

        favoriteDto.Should().NotBeNull();
        favoriteDto!.UserId.Should().Be(user.Id);
        favoriteDto.MovieId.Should().Be(movie.Id);
        favoriteDto.MovieTitle.Should().Be("Awesome Movie");

        // Verify in database
        var dbFavorite = await DbContext.Favorites.FindAsync(favoriteDto.Id);
        dbFavorite.Should().NotBeNull();
        dbFavorite!.MovieId.Should().Be(movie.Id);
    }

    [Fact]
    public async Task AddToFavorites_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        await ClearDatabaseAsync();
        var movie = await CreateTestMovieAsync();
        ClearAuthorizationHeader();

        var addRequest = new AddToFavoritesRequest
        {
            MovieId = movie.Id
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/favorites", addRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddToFavorites_NonExistentMovie_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestUserAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var addRequest = new AddToFavoritesRequest
        {
            MovieId = Guid.NewGuid()
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/favorites", addRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddToFavorites_AlreadyInFavorites_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        await CreateTestFavoriteAsync(user.Id, movie.Id);
        
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var addRequest = new AddToFavoritesRequest
        {
            MovieId = movie.Id
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/favorites", addRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoveFromFavorites_WithExistingFavorite_ShouldRemoveFromFavorites()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var favorite = await CreateTestFavoriteAsync(user.Id, movie.Id);
        
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.DeleteAsync($"/api/favorites/{movie.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion in database
        RefreshDbContext();
        var dbFavorite = await DbContext.Favorites.FindAsync(favorite.Id);
        dbFavorite.Should().BeNull();
    }

    [Fact]
    public async Task RemoveFromFavorites_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        await ClearDatabaseAsync();
        var movie = await CreateTestMovieAsync();
        ClearAuthorizationHeader();

        // Act
        var response = await Client.DeleteAsync($"/api/favorites/{movie.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RemoveFromFavorites_NotInFavorites_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.DeleteAsync($"/api/favorites/{movie.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CheckIsFavorite_WithFavoriteMovie_ShouldReturnTrue()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        await CreateTestFavoriteAsync(user.Id, movie.Id);
        
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.GetAsync($"/api/favorites/check/{movie.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);

        jsonDoc.RootElement.GetProperty("movieId").GetGuid().Should().Be(movie.Id);
        jsonDoc.RootElement.GetProperty("isFavorite").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task CheckIsFavorite_WithNonFavoriteMovie_ShouldReturnFalse()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.GetAsync($"/api/favorites/check/{movie.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);

        jsonDoc.RootElement.GetProperty("movieId").GetGuid().Should().Be(movie.Id);
        jsonDoc.RootElement.GetProperty("isFavorite").GetBoolean().Should().BeFalse();
    }

    [Fact]
    public async Task CheckIsFavorite_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        await ClearDatabaseAsync();
        var movie = await CreateTestMovieAsync();
        ClearAuthorizationHeader();

        // Act
        var response = await Client.GetAsync($"/api/favorites/check/{movie.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task FavoritesFlow_CompleteFlow_ShouldWorkCorrectly()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie1 = await CreateTestMovieAsync("Movie 1");
        var movie2 = await CreateTestMovieAsync("Movie 2");
        
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Act & Assert

        // 1. Check favorites (should be empty)
        var getFavoritesResponse1 = await Client.GetAsync("/api/favorites");
        getFavoritesResponse1.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content1 = await getFavoritesResponse1.Content.ReadAsStringAsync();
        var jsonDoc1 = JsonDocument.Parse(content1);
        var favorites1 = JsonSerializer.Deserialize<List<FavoriteDto>>(
            jsonDoc1.RootElement.GetProperty("data").GetRawText(),
            JsonOptions);
        favorites1.Should().BeEmpty();

        // 2. Check if movie1 is favorite (should be false)
        var checkResponse1 = await Client.GetAsync($"/api/favorites/check/{movie1.Id}");
        checkResponse1.StatusCode.Should().Be(HttpStatusCode.OK);
        var checkContent1 = await checkResponse1.Content.ReadAsStringAsync();
        var checkDoc1 = JsonDocument.Parse(checkContent1);
        checkDoc1.RootElement.GetProperty("isFavorite").GetBoolean().Should().BeFalse();

        // 3. Add movie1 to favorites
        var addRequest1 = new AddToFavoritesRequest { MovieId = movie1.Id };
        var addResponse1 = await Client.PostAsJsonAsync("/api/favorites", addRequest1);
        addResponse1.StatusCode.Should().Be(HttpStatusCode.Created);

        // 4. Add movie2 to favorites
        var addRequest2 = new AddToFavoritesRequest { MovieId = movie2.Id };
        var addResponse2 = await Client.PostAsJsonAsync("/api/favorites", addRequest2);
        addResponse2.StatusCode.Should().Be(HttpStatusCode.Created);

        // 5. Check favorites (should have 2 items)
        var getFavoritesResponse2 = await Client.GetAsync("/api/favorites");
        var content2 = await getFavoritesResponse2.Content.ReadAsStringAsync();
        var jsonDoc2 = JsonDocument.Parse(content2);
        var favorites2 = JsonSerializer.Deserialize<List<FavoriteDto>>(
            jsonDoc2.RootElement.GetProperty("data").GetRawText(),
            JsonOptions);
        favorites2.Should().HaveCount(2);

        // 6. Check if movie1 is favorite (should be true)
        var checkResponse2 = await Client.GetAsync($"/api/favorites/check/{movie1.Id}");
        var checkContent2 = await checkResponse2.Content.ReadAsStringAsync();
        var checkDoc2 = JsonDocument.Parse(checkContent2);
        checkDoc2.RootElement.GetProperty("isFavorite").GetBoolean().Should().BeTrue();

        // 7. Remove movie1 from favorites
        var removeResponse = await Client.DeleteAsync($"/api/favorites/{movie1.Id}");
        removeResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 8. Check favorites (should have 1 item)
        var getFavoritesResponse3 = await Client.GetAsync("/api/favorites");
        var content3 = await getFavoritesResponse3.Content.ReadAsStringAsync();
        var jsonDoc3 = JsonDocument.Parse(content3);
        var favorites3 = JsonSerializer.Deserialize<List<FavoriteDto>>(
            jsonDoc3.RootElement.GetProperty("data").GetRawText(),
            JsonOptions);
        favorites3.Should().HaveCount(1);
        favorites3.First().MovieTitle.Should().Be("Movie 2");

        // 9. Check if movie1 is favorite (should be false again)
        var checkResponse3 = await Client.GetAsync($"/api/favorites/check/{movie1.Id}");
        var checkContent3 = await checkResponse3.Content.ReadAsStringAsync();
        var checkDoc3 = JsonDocument.Parse(checkContent3);
        checkDoc3.RootElement.GetProperty("isFavorite").GetBoolean().Should().BeFalse();
    }

    [Fact]
    public async Task GetUserFavorites_WithLargePageSize_ShouldLimitToMaximum()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Create one favorite
        var movie = await CreateTestMovieAsync();
        await CreateTestFavoriteAsync(user.Id, movie.Id);

        // Act
        var response = await Client.GetAsync("/api/favorites?pageSize=100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        var pagination = jsonDoc.RootElement.GetProperty("pagination");

        pagination.GetProperty("pageSize").GetInt32().Should().Be(50); // Should be limited to 50
    }

    private async Task<Favorite> CreateTestFavoriteAsync(Guid userId, Guid movieId)
    {
        var favorite = new Favorite
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MovieId = movieId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        DbContext.Favorites.Add(favorite);
        await DbContext.SaveChangesAsync();
        return favorite;
    }
}