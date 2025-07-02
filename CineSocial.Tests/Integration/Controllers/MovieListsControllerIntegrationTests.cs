using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CineSocial.Api.DTOs;
using CineSocial.Domain.Entities;
using CineSocial.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Tests.Integration.Controllers;

public class MovieListsControllerIntegrationTests : IntegrationTestBase
{
    public MovieListsControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetPublicMovieLists_ShouldReturnPublicLists()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        
        var publicList = await CreateTestMovieListAsync(user.Id, "Public List", "Description", true, false);
        var privateList = await CreateTestMovieListAsync(user.Id, "Private List", "Description", false, false);

        // Act
        var response = await Client.GetAsync("/api/movie-lists/public");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        var movieLists = JsonSerializer.Deserialize<List<UserMovieListDto>>(
            jsonDoc.RootElement.GetProperty("movieLists").GetRawText(), 
            JsonOptions);

        movieLists.Should().NotBeNull();
        movieLists!.Should().HaveCount(1);
        movieLists.First().Name.Should().Be("Public List");
        movieLists.First().IsPublic.Should().BeTrue();
    }

    [Fact]
    public async Task GetPublicMovieLists_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        
        // Create 5 public lists
        for (int i = 1; i <= 5; i++)
        {
            await CreateTestMovieListAsync(user.Id, $"Public List {i}", $"Description {i}", true, false);
        }

        // Act
        var response = await Client.GetAsync("/api/movie-lists/public?page=1&pageSize=3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        
        var movieLists = JsonSerializer.Deserialize<List<UserMovieListDto>>(
            jsonDoc.RootElement.GetProperty("movieLists").GetRawText(), 
            JsonOptions);
        
        var pagination = jsonDoc.RootElement;

        movieLists.Should().HaveCount(3);
        pagination.GetProperty("totalCount").GetInt32().Should().Be(5);
        pagination.GetProperty("page").GetInt32().Should().Be(1);
        pagination.GetProperty("pageSize").GetInt32().Should().Be(3);
        pagination.GetProperty("totalPages").GetInt32().Should().Be(2);
    }

    [Fact]
    public async Task GetPublicMovieLists_WithSearch_ShouldReturnFilteredResults()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        
        await CreateTestMovieListAsync(user.Id, "Sci-Fi Movies", "Science fiction films", true, false);
        await CreateTestMovieListAsync(user.Id, "Action Movies", "Action packed films", true, false);
        await CreateTestMovieListAsync(user.Id, "Drama Movies", "Dramatic films", true, false);

        // Act
        var response = await Client.GetAsync("/api/movie-lists/public?search=sci-fi");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        var movieLists = JsonSerializer.Deserialize<List<UserMovieListDto>>(
            jsonDoc.RootElement.GetProperty("movieLists").GetRawText(), 
            JsonOptions);

        movieLists.Should().NotBeNull();
        movieLists!.Should().HaveCount(1);
        movieLists.First().Name.Should().Be("Sci-Fi Movies");
    }

    [Fact]
    public async Task GetMovieListById_WithValidId_ShouldReturnListDetails()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var movieList = await CreateTestMovieListAsync(user.Id, "Test List", "Description", true, false);
        await CreateTestMovieListItemAsync(movieList.Id, movie.Id, "Great movie!", 1);

        // Act
        var response = await Client.GetAsync($"/api/movie-lists/{movieList.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var listDetail = JsonSerializer.Deserialize<MovieListDetailDto>(content, JsonOptions);

        listDetail.Should().NotBeNull();
        listDetail!.Id.Should().Be(movieList.Id);
        listDetail.Name.Should().Be("Test List");
        listDetail.Movies.Should().HaveCount(1);
        listDetail.Movies.First().MovieTitle.Should().Be(movie.Title);
        listDetail.Movies.First().Notes.Should().Be("Great movie!");
    }

    [Fact]
    public async Task GetMovieListById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/movie-lists/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMovieListById_WithPrivateListAsNonOwner_ShouldReturnForbidden()
    {
        // Arrange
        await ClearDatabaseAsync();
        var owner = await CreateTestUserAsync("owner@example.com", "owner");
        var movieList = await CreateTestMovieListAsync(owner.Id, "Private List", "Description", false, false);

        // Act
        var response = await Client.GetAsync($"/api/movie-lists/{movieList.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound); // Should be treated as not found for security
    }

    [Fact]
    public async Task GetUserMovieLists_WithValidUserId_ShouldReturnUserLists()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var watchlist = await CreateTestMovieListAsync(user.Id, "Watchlist", "My watchlist", false, true);
        var customList = await CreateTestMovieListAsync(user.Id, "Favorites", "My favorites", true, false);

        // Act
        var response = await Client.GetAsync($"/api/movie-lists/user/{user.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var movieLists = JsonSerializer.Deserialize<List<UserMovieListDto>>(content, JsonOptions);

        movieLists.Should().NotBeNull();
        movieLists!.Should().HaveCount(2);
        movieLists.Should().Contain(ml => ml.IsWatchlist);
        movieLists.Should().Contain(ml => !ml.IsWatchlist);
    }

    [Fact]
    public async Task CreateMovieList_WithValidData_ShouldCreateList()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var createRequest = new CreateMovieListRequest
        {
            Name = "My Sci-Fi Collection",
            Description = "Best science fiction movies",
            IsPublic = true
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/movie-lists", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        var movieList = JsonSerializer.Deserialize<UserMovieListDto>(content, JsonOptions);

        movieList.Should().NotBeNull();
        movieList!.Name.Should().Be("My Sci-Fi Collection");
        movieList.Description.Should().Be("Best science fiction movies");
        movieList.IsPublic.Should().BeTrue();
        movieList.UserId.Should().Be(user.Id);
        movieList.MovieCount.Should().Be(0);

        // Verify in database
        var dbList = await DbContext.MovieLists.FindAsync(movieList.Id);
        dbList.Should().NotBeNull();
        dbList!.Name.Should().Be("My Sci-Fi Collection");
    }

    [Fact]
    public async Task CreateMovieList_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        await ClearDatabaseAsync();
        ClearAuthorizationHeader();

        var createRequest = new CreateMovieListRequest
        {
            Name = "Test List",
            Description = "Test description",
            IsPublic = true
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/movie-lists", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateMovieList_WithDuplicateName_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Create first list
        await CreateTestMovieListAsync(user.Id, "My Collection", "Description", true, false);

        var createRequest = new CreateMovieListRequest
        {
            Name = "My Collection", // Same name
            Description = "Another description",
            IsPublic = true
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/movie-lists", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateWatchlist_ShouldCreateOrReturnExistingWatchlist()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.PostAsync("/api/movie-lists/watchlist", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        var watchlist = JsonSerializer.Deserialize<UserMovieListDto>(content, JsonOptions);

        watchlist.Should().NotBeNull();
        watchlist!.Name.Should().Be("İzleme Listesi");
        watchlist.IsWatchlist.Should().BeTrue();
        watchlist.UserId.Should().Be(user.Id);

        // Verify in database
        var dbWatchlist = await DbContext.MovieLists.FindAsync(watchlist.Id);
        dbWatchlist.Should().NotBeNull();
        dbWatchlist!.IsWatchlist.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateMovieList_WithValidData_ShouldUpdateList()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movieList = await CreateTestMovieListAsync(user.Id, "Original Name", "Original description", true, false);
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var updateRequest = new UpdateMovieListRequest
        {
            Name = "Updated Name",
            Description = "Updated description",
            IsPublic = false
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/movie-lists/{movieList.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var updatedList = JsonSerializer.Deserialize<UserMovieListDto>(content, JsonOptions);

        updatedList.Should().NotBeNull();
        updatedList!.Name.Should().Be("Updated Name");
        updatedList.Description.Should().Be("Updated description");
        updatedList.IsPublic.Should().BeFalse();

        // Verify in database
        RefreshDbContext();
        var dbList = await DbContext.MovieLists.FindAsync(movieList.Id);
        dbList.Should().NotBeNull();
        dbList!.Name.Should().Be("Updated Name");
        dbList.IsPublic.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteMovieList_WithValidId_ShouldDeleteList()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var movieList = await CreateTestMovieListAsync(user.Id, "To Delete", "Description", true, false);
        await CreateTestMovieListItemAsync(movieList.Id, movie.Id, "Notes", 1);
        
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.DeleteAsync($"/api/movie-lists/{movieList.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion in database
        RefreshDbContext();
        var dbList = await DbContext.MovieLists.FindAsync(movieList.Id);
        dbList.Should().BeNull();

        // Verify list items are also deleted
        var dbItems = DbContext.MovieListItems.Where(mli => mli.MovieListId == movieList.Id).ToList();
        dbItems.Should().BeEmpty();
    }

    [Fact]
    public async Task AddMovieToList_WithValidData_ShouldAddMovie()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var movieList = await CreateTestMovieListAsync(user.Id, "Test List", "Description", true, false);
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var addRequest = new AddMovieToListRequest
        {
            MovieId = movie.Id,
            Notes = "Amazing movie!"
        };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/movie-lists/{movieList.Id}/movies", addRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        var movieListItem = JsonSerializer.Deserialize<MovieListItemDto>(content, JsonOptions);

        movieListItem.Should().NotBeNull();
        movieListItem!.MovieId.Should().Be(movie.Id);
        movieListItem.MovieTitle.Should().Be(movie.Title);
        movieListItem.Notes.Should().Be("Amazing movie!");
        movieListItem.Order.Should().Be(1);

        // Verify in database
        var dbItem = await DbContext.MovieListItems
            .FirstOrDefaultAsync(mli => mli.MovieListId == movieList.Id && mli.MovieId == movie.Id);
        dbItem.Should().NotBeNull();
        dbItem!.Notes.Should().Be("Amazing movie!");
    }

    [Fact]
    public async Task RemoveMovieFromList_WithValidData_ShouldRemoveMovie()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var movieList = await CreateTestMovieListAsync(user.Id, "Test List", "Description", true, false);
        await CreateTestMovieListItemAsync(movieList.Id, movie.Id, "Notes", 1);
        
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.DeleteAsync($"/api/movie-lists/{movieList.Id}/movies/{movie.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion in database
        RefreshDbContext();
        var dbItem = await DbContext.MovieListItems
            .FirstOrDefaultAsync(mli => mli.MovieListId == movieList.Id && mli.MovieId == movie.Id);
        dbItem.Should().BeNull();
    }

    [Fact]
    public async Task AddListToFavorites_WithValidPublicList_ShouldAddToFavorites()
    {
        // Arrange
        await ClearDatabaseAsync();
        var listOwner = await CreateTestUserAsync("owner@example.com", "owner");
        var currentUser = await CreateTestUserAsync("user@example.com", "user");
        var movieList = await CreateTestMovieListAsync(listOwner.Id, "Public List", "Description", true, false);
        
        var authResponse = await LoginUserAsync("user@example.com", "password123");
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.PostAsync($"/api/movie-lists/{movieList.Id}/favorite", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify in database
        var dbFavorite = await DbContext.ListFavorites
            .FirstOrDefaultAsync(lf => lf.UserId == currentUser.Id && lf.MovieListId == movieList.Id);
        dbFavorite.Should().NotBeNull();
    }

    [Fact]
    public async Task RemoveListFromFavorites_WithExistingFavorite_ShouldRemoveFromFavorites()
    {
        // Arrange
        await ClearDatabaseAsync();
        var listOwner = await CreateTestUserAsync("owner@example.com", "owner");
        var currentUser = await CreateTestUserAsync("user@example.com", "user");
        var movieList = await CreateTestMovieListAsync(listOwner.Id, "Public List", "Description", true, false);
        
        // Add to favorites first
        DbContext.ListFavorites.Add(new ListFavorite
        {
            Id = Guid.NewGuid(),
            UserId = currentUser.Id,
            MovieListId = movieList.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await DbContext.SaveChangesAsync();
        
        var authResponse = await LoginUserAsync("user@example.com", "password123");
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.DeleteAsync($"/api/movie-lists/{movieList.Id}/favorite");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion in database
        RefreshDbContext();
        var dbFavorite = await DbContext.ListFavorites
            .FirstOrDefaultAsync(lf => lf.UserId == currentUser.Id && lf.MovieListId == movieList.Id);
        dbFavorite.Should().BeNull();
    }

    private async Task<MovieList> CreateTestMovieListAsync(Guid userId, string name, string description, bool isPublic, bool isWatchlist)
    {
        var movieList = new MovieList
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            Description = description,
            IsPublic = isPublic,
            IsWatchlist = isWatchlist,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        DbContext.MovieLists.Add(movieList);
        await DbContext.SaveChangesAsync();
        return movieList;
    }

    private async Task<MovieListItem> CreateTestMovieListItemAsync(Guid movieListId, Guid movieId, string notes, int order)
    {
        var movieListItem = new MovieListItem
        {
            Id = Guid.NewGuid(),
            MovieListId = movieListId,
            MovieId = movieId,
            Notes = notes,
            Order = order,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        DbContext.MovieListItems.Add(movieListItem);
        await DbContext.SaveChangesAsync();
        return movieListItem;
    }
}