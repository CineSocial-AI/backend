using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CineSocial.Api.DTOs;
using CineSocial.Domain.Entities;
using CineSocial.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CineSocial.Tests.Integration.Controllers;

public class ReviewsControllerIntegrationTests : IntegrationTestBase
{
    public ReviewsControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetMovieReviews_WithValidMovieId_ShouldReturnReviews()
    {
        // Arrange
        await ClearDatabaseAsync();
        var movie = await CreateTestMovieAsync();
        var user = await CreateTestUserAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);

        // Act
        var response = await Client.GetAsync($"/api/reviews/movie/{movie.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        var reviews = JsonSerializer.Deserialize<List<ReviewDto>>(
            jsonDoc.RootElement.GetProperty("data").GetRawText(), 
            JsonOptions);

        reviews.Should().NotBeNull();
        reviews!.Should().HaveCount(1);
        reviews.First().Id.Should().Be(review.Id);
        reviews.First().Title.Should().Be(review.Title);
        reviews.First().Content.Should().Be(review.Content);
    }

    [Fact]
    public async Task GetMovieReviews_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await ClearDatabaseAsync();
        var movie = await CreateTestMovieAsync();
        var user = await CreateTestUserAsync();
        
        // Create multiple reviews
        for (int i = 1; i <= 15; i++)
        {
            await CreateTestReviewAsync(user.Id, movie.Id, $"Review {i}", $"Content {i}");
        }

        // Act
        var response = await Client.GetAsync($"/api/reviews/movie/{movie.Id}?page=2&pageSize=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        
        var reviews = JsonSerializer.Deserialize<List<ReviewDto>>(
            jsonDoc.RootElement.GetProperty("data").GetRawText(), 
            JsonOptions);
        
        var pagination = jsonDoc.RootElement.GetProperty("pagination");

        reviews.Should().HaveCount(5);
        pagination.GetProperty("currentPage").GetInt32().Should().Be(2);
        pagination.GetProperty("pageSize").GetInt32().Should().Be(5);
        pagination.GetProperty("totalCount").GetInt32().Should().Be(15);
    }

    [Fact]
    public async Task GetMovieReviews_WithSorting_ShouldReturnSortedResults()
    {
        // Arrange
        await ClearDatabaseAsync();
        var movie = await CreateTestMovieAsync();
        var user = await CreateTestUserAsync();
        
        var review1 = await CreateTestReviewAsync(user.Id, movie.Id, "A Review", "Content A");
        var review2 = await CreateTestReviewAsync(user.Id, movie.Id, "C Review", "Content C");
        var review3 = await CreateTestReviewAsync(user.Id, movie.Id, "B Review", "Content B");

        // Act
        var response = await Client.GetAsync($"/api/reviews/movie/{movie.Id}?sortBy=created_at&sortOrder=asc");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        var reviews = JsonSerializer.Deserialize<List<ReviewDto>>(
            jsonDoc.RootElement.GetProperty("data").GetRawText(), 
            JsonOptions);

        reviews.Should().HaveCount(3);
        reviews.First().Id.Should().Be(review1.Id);
        reviews.Last().Id.Should().Be(review3.Id);
    }

    [Fact]
    public async Task GetReview_WithValidId_ShouldReturnReview()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);

        // Act
        var response = await Client.GetAsync($"/api/reviews/{review.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var reviewDto = JsonSerializer.Deserialize<ReviewDto>(content, JsonOptions);

        reviewDto.Should().NotBeNull();
        reviewDto!.Id.Should().Be(review.Id);
        reviewDto.Title.Should().Be(review.Title);
        reviewDto.Content.Should().Be(review.Content);
        reviewDto.UserFullName.Should().Be($"{user.FirstName} {user.LastName}");
        reviewDto.UserUsername.Should().Be(user.Username);
    }

    [Fact]
    public async Task GetReview_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/reviews/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateReview_WithValidData_ShouldCreateReview()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var createRequest = new CreateReviewRequest
        {
            MovieId = movie.Id,
            Title = "Great Movie Review",
            Content = "This is a comprehensive review of a great movie.",
            ContainsSpoilers = false
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/reviews", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        var reviewDto = JsonSerializer.Deserialize<ReviewDto>(content, JsonOptions);

        reviewDto.Should().NotBeNull();
        reviewDto!.MovieId.Should().Be(movie.Id);
        reviewDto.UserId.Should().Be(user.Id);
        reviewDto.Title.Should().Be(createRequest.Title);
        reviewDto.Content.Should().Be(createRequest.Content);
        reviewDto.ContainsSpoilers.Should().Be(createRequest.ContainsSpoilers);

        // Verify in database
        var dbReview = await DbContext.Reviews.FindAsync(reviewDto.Id);
        dbReview.Should().NotBeNull();
        dbReview!.Title.Should().Be(createRequest.Title);
    }

    [Fact]
    public async Task CreateReview_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        await ClearDatabaseAsync();
        var movie = await CreateTestMovieAsync();
        ClearAuthorizationHeader();

        var createRequest = new CreateReviewRequest
        {
            MovieId = movie.Id,
            Title = "Great Movie Review",
            Content = "This is a comprehensive review of a great movie."
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/reviews", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateReview_ForNonExistentMovie_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestUserAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var createRequest = new CreateReviewRequest
        {
            MovieId = Guid.NewGuid(),
            Title = "Great Movie Review",
            Content = "This is a comprehensive review of a great movie."
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/reviews", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateReview_DuplicateReview_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Create first review
        await CreateTestReviewAsync(user.Id, movie.Id);

        var createRequest = new CreateReviewRequest
        {
            MovieId = movie.Id,
            Title = "Another Review",
            Content = "This is another review for the same movie."
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/reviews", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("", "Valid content that is long enough")]
    [InlineData("Hi", "Valid content that is long enough")]
    [InlineData("Valid title", "")]
    [InlineData("Valid title", "Short")]
    public async Task CreateReview_WithInvalidData_ShouldReturnBadRequest(string title, string content)
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var createRequest = new CreateReviewRequest
        {
            MovieId = movie.Id,
            Title = title,
            Content = content
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/reviews", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateReview_WithValidData_ShouldUpdateReview()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var updateRequest = new UpdateReviewRequest
        {
            Title = "Updated Review Title",
            Content = "This is the updated content of the review.",
            ContainsSpoilers = true
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/reviews/{review.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var reviewDto = JsonSerializer.Deserialize<ReviewDto>(content, JsonOptions);

        reviewDto.Should().NotBeNull();
        reviewDto!.Title.Should().Be(updateRequest.Title);
        reviewDto.Content.Should().Be(updateRequest.Content);
        reviewDto.ContainsSpoilers.Should().Be(updateRequest.ContainsSpoilers);
        reviewDto.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateReview_NotOwner_ShouldReturnForbidden()
    {
        // Arrange
        await ClearDatabaseAsync();
        var reviewOwner = await CreateTestUserAsync("owner@example.com", "owner");
        var otherUser = await CreateTestUserAsync("other@example.com", "other");
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(reviewOwner.Id, movie.Id);
        
        // Login as other user
        var authResponse = await LoginUserAsync("other@example.com", "password123");
        SetAuthorizationHeader(authResponse.Token);

        var updateRequest = new UpdateReviewRequest
        {
            Title = "Malicious Update",
            Content = "Trying to update someone else's review."
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/reviews/{review.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteReview_WithValidId_ShouldDeleteReview()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.DeleteAsync($"/api/reviews/{review.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion in database
        RefreshDbContext();
        var dbReview = await DbContext.Reviews.FindAsync(review.Id);
        dbReview.Should().BeNull();
    }

    [Fact]
    public async Task DeleteReview_NotOwner_ShouldReturnForbidden()
    {
        // Arrange
        await ClearDatabaseAsync();
        var reviewOwner = await CreateTestUserAsync("owner@example.com", "owner");
        var otherUser = await CreateTestUserAsync("other@example.com", "other");
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(reviewOwner.Id, movie.Id);
        
        // Login as other user
        var authResponse = await LoginUserAsync("other@example.com", "password123");
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.DeleteAsync($"/api/reviews/{review.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RateMovie_WithValidData_ShouldCreateRating()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var ratingRequest = new CreateRatingRequest
        {
            MovieId = movie.Id,
            Score = 8
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/reviews/rating", ratingRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        var ratingDto = JsonSerializer.Deserialize<RatingDto>(content, JsonOptions);

        ratingDto.Should().NotBeNull();
        ratingDto!.MovieId.Should().Be(movie.Id);
        ratingDto.UserId.Should().Be(user.Id);
        ratingDto.Score.Should().Be(8);
    }

    [Fact]
    public async Task RateMovie_UpdateExistingRating_ShouldReturnOk()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Create initial rating
        var initialRating = new CreateRatingRequest
        {
            MovieId = movie.Id,
            Score = 6
        };
        await Client.PostAsJsonAsync("/api/reviews/rating", initialRating);

        // Update rating
        var updatedRating = new CreateRatingRequest
        {
            MovieId = movie.Id,
            Score = 9
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/reviews/rating", updatedRating);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var ratingDto = JsonSerializer.Deserialize<RatingDto>(content, JsonOptions);

        ratingDto.Should().NotBeNull();
        ratingDto!.Score.Should().Be(9);
        ratingDto.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public async Task RateMovie_WithInvalidScore_ShouldReturnBadRequest(int score)
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var ratingRequest = new CreateRatingRequest
        {
            MovieId = movie.Id,
            Score = score
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/reviews/rating", ratingRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetUserRating_WithExistingRating_ShouldReturnRating()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Create rating first
        var ratingRequest = new CreateRatingRequest
        {
            MovieId = movie.Id,
            Score = 7
        };
        await Client.PostAsJsonAsync("/api/reviews/rating", ratingRequest);

        // Act
        var response = await Client.GetAsync($"/api/reviews/rating/movie/{movie.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var ratingDto = JsonSerializer.Deserialize<RatingDto>(content, JsonOptions);

        ratingDto.Should().NotBeNull();
        ratingDto!.Score.Should().Be(7);
    }

    [Fact]
    public async Task GetUserRating_WithoutRating_ShouldReturnNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.GetAsync($"/api/reviews/rating/movie/{movie.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMovieRatingStats_ShouldReturnStats()
    {
        // Arrange
        await ClearDatabaseAsync();
        var movie = await CreateTestMovieAsync();
        var user1 = await CreateTestUserAsync("user1@example.com", "user1");
        var user2 = await CreateTestUserAsync("user2@example.com", "user2");

        // Create ratings
        DbContext.Ratings.Add(new Rating
        {
            Id = Guid.NewGuid(),
            UserId = user1.Id,
            MovieId = movie.Id,
            Score = 8,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        DbContext.Ratings.Add(new Rating
        {
            Id = Guid.NewGuid(),
            UserId = user2.Id,
            MovieId = movie.Id,
            Score = 6,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await DbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/reviews/rating/movie/{movie.Id}/stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);

        jsonDoc.RootElement.GetProperty("movieId").GetGuid().Should().Be(movie.Id);
        jsonDoc.RootElement.GetProperty("totalRatings").GetInt32().Should().Be(2);
        jsonDoc.RootElement.GetProperty("averageRating").GetDouble().Should().Be(7.0);
    }

    [Fact]
    public async Task DeleteRating_WithExistingRating_ShouldDeleteRating()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Create rating first
        var ratingRequest = new CreateRatingRequest
        {
            MovieId = movie.Id,
            Score = 5
        };
        await Client.PostAsJsonAsync("/api/reviews/rating", ratingRequest);

        // Act
        var response = await Client.DeleteAsync($"/api/reviews/rating/movie/{movie.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getRatingResponse = await Client.GetAsync($"/api/reviews/rating/movie/{movie.Id}");
        getRatingResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<Review> CreateTestReviewAsync(Guid userId, Guid movieId, string title = "Test Review", string content = "This is a test review content.")
    {
        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MovieId = movieId,
            Title = title,
            Content = content,
            ContainsSpoilers = false,
            LikesCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        DbContext.Reviews.Add(review);
        await DbContext.SaveChangesAsync();
        return review;
    }
}