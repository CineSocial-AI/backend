using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CineSocial.Api.DTOs;
using CineSocial.Domain.Entities;
using CineSocial.Domain.Enums;
using CineSocial.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CineSocial.Tests.Integration.Controllers;

public class CommentsControllerIntegrationTests : IntegrationTestBase
{
    public CommentsControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetReviewComments_WithValidReviewId_ShouldReturnComments()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);
        var comment = await CreateTestCommentAsync(user.Id, review.Id);

        // Act
        var response = await Client.GetAsync($"/api/comments/review/{review.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        var comments = JsonSerializer.Deserialize<List<CommentDto>>(
            jsonDoc.RootElement.GetProperty("data").GetRawText(),
            JsonOptions);

        comments.Should().NotBeNull();
        comments!.Should().HaveCount(1);
        comments.First().Id.Should().Be(comment.Id);
        comments.First().Content.Should().Be(comment.Content);
        comments.First().Username.Should().Be(user.Username);
    }

    [Fact]
    public async Task GetReviewComments_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);

        // Create multiple comments
        for (int i = 1; i <= 15; i++)
        {
            await CreateTestCommentAsync(user.Id, review.Id, $"Comment {i}");
        }

        // Act
        var response = await Client.GetAsync($"/api/comments/review/{review.Id}?page=2&pageSize=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);

        var comments = JsonSerializer.Deserialize<List<CommentDto>>(
            jsonDoc.RootElement.GetProperty("data").GetRawText(),
            JsonOptions);

        var pagination = jsonDoc.RootElement.GetProperty("pagination");

        comments.Should().HaveCount(5);
        pagination.GetProperty("currentPage").GetInt32().Should().Be(2);
        pagination.GetProperty("pageSize").GetInt32().Should().Be(5);
        pagination.GetProperty("totalCount").GetInt32().Should().Be(15);
    }

    [Fact]
    public async Task GetReviewComments_WithNestedReplies_ShouldReturnCommentsWithReplies()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);
        var parentComment = await CreateTestCommentAsync(user.Id, review.Id, "Parent comment");
        var replyComment = await CreateTestCommentAsync(user.Id, review.Id, "Reply comment", parentComment.Id);

        // Act
        var response = await Client.GetAsync($"/api/comments/review/{review.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        var comments = JsonSerializer.Deserialize<List<CommentDto>>(
            jsonDoc.RootElement.GetProperty("data").GetRawText(),
            JsonOptions);

        comments.Should().NotBeNull();
        var parentCommentDto = comments!.First(c => c.Id == parentComment.Id);
        parentCommentDto.Replies.Should().HaveCount(1);
        parentCommentDto.Replies.First().Id.Should().Be(replyComment.Id);
        parentCommentDto.Replies.First().Content.Should().Be("Reply comment");
    }

    [Fact]
    public async Task GetComment_WithValidId_ShouldReturnComment()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);
        var comment = await CreateTestCommentAsync(user.Id, review.Id);

        // Act
        var response = await Client.GetAsync($"/api/comments/{comment.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var commentDto = JsonSerializer.Deserialize<CommentDto>(content, JsonOptions);

        commentDto.Should().NotBeNull();
        commentDto!.Id.Should().Be(comment.Id);
        commentDto.Content.Should().Be(comment.Content);
        commentDto.Username.Should().Be(user.Username);
    }

    [Fact]
    public async Task GetComment_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/comments/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateComment_WithValidData_ShouldCreateComment()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var createRequest = new CreateCommentRequest
        {
            ReviewId = review.Id,
            Content = "This is a great review comment."
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/comments", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        var commentDto = JsonSerializer.Deserialize<CommentDto>(content, JsonOptions);

        commentDto.Should().NotBeNull();
        commentDto!.ReviewId.Should().Be(review.Id);
        commentDto.UserId.Should().Be(user.Id);
        commentDto.Content.Should().Be(createRequest.Content);
        commentDto.Username.Should().Be(user.Username);

        // Verify in database
        var dbComment = await DbContext.Comments.FindAsync(commentDto.Id);
        dbComment.Should().NotBeNull();
        dbComment!.Content.Should().Be(createRequest.Content);
    }

    [Fact]
    public async Task CreateComment_AsReply_ShouldCreateNestedComment()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);
        var parentComment = await CreateTestCommentAsync(user.Id, review.Id);
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var createRequest = new CreateCommentRequest
        {
            ReviewId = review.Id,
            Content = "This is a reply to the comment.",
            ParentCommentId = parentComment.Id
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/comments", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        var commentDto = JsonSerializer.Deserialize<CommentDto>(content, JsonOptions);

        commentDto.Should().NotBeNull();
        commentDto!.ParentCommentId.Should().Be(parentComment.Id);
        commentDto.Content.Should().Be(createRequest.Content);
    }

    [Fact]
    public async Task CreateComment_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);
        ClearAuthorizationHeader();

        var createRequest = new CreateCommentRequest
        {
            ReviewId = review.Id,
            Content = "This comment should not be created."
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/comments", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("")]
    [InlineData("X")]
    public async Task CreateComment_WithInvalidContent_ShouldReturnBadRequest(string content)
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var createRequest = new CreateCommentRequest
        {
            ReviewId = review.Id,
            Content = content
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/comments", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateComment_WithValidData_ShouldUpdateComment()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);
        var comment = await CreateTestCommentAsync(user.Id, review.Id);
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var updateRequest = new UpdateCommentRequest
        {
            Content = "This is the updated comment content."
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/comments/{comment.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var commentDto = JsonSerializer.Deserialize<CommentDto>(content, JsonOptions);

        commentDto.Should().NotBeNull();
        commentDto!.Content.Should().Be(updateRequest.Content);
        commentDto.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateComment_NotOwner_ShouldReturnForbidden()
    {
        // Arrange
        await ClearDatabaseAsync();
        var commentOwner = await CreateTestUserAsync("owner@example.com", "owner");
        var otherUser = await CreateTestUserAsync("other@example.com", "other");
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(commentOwner.Id, movie.Id);
        var comment = await CreateTestCommentAsync(commentOwner.Id, review.Id);

        // Login as other user
        var authResponse = await LoginUserAsync("other@example.com", "password123");
        SetAuthorizationHeader(authResponse.Token);

        var updateRequest = new UpdateCommentRequest
        {
            Content = "Malicious update attempt."
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/comments/{comment.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteComment_WithValidId_ShouldDeleteComment()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);
        var comment = await CreateTestCommentAsync(user.Id, review.Id);
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.DeleteAsync($"/api/comments/{comment.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion in database
        RefreshDbContext();
        var dbComment = await DbContext.Comments.FindAsync(comment.Id);
        dbComment.Should().BeNull();
    }

    [Fact]
    public async Task DeleteComment_NotOwner_ShouldReturnForbidden()
    {
        // Arrange
        await ClearDatabaseAsync();
        var commentOwner = await CreateTestUserAsync("owner@example.com", "owner");
        var otherUser = await CreateTestUserAsync("other@example.com", "other");
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(commentOwner.Id, movie.Id);
        var comment = await CreateTestCommentAsync(commentOwner.Id, review.Id);

        // Login as other user
        var authResponse = await LoginUserAsync("other@example.com", "password123");
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.DeleteAsync($"/api/comments/{comment.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ReactToComment_WithUpvote_ShouldCreateReaction()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);
        var comment = await CreateTestCommentAsync(user.Id, review.Id);
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        var reactionRequest = new CreateReactionRequest
        {
            CommentId = comment.Id,
            Type = ReactionType.Upvote
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/comments/reaction", reactionRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        var reactionDto = JsonSerializer.Deserialize<ReactionDto>(content, JsonOptions);

        reactionDto.Should().NotBeNull();
        reactionDto!.CommentId.Should().Be(comment.Id);
        reactionDto.UserId.Should().Be(user.Id);
        reactionDto.Type.Should().Be(ReactionType.Upvote);
    }

    [Fact]
    public async Task ReactToComment_UpdateExistingReaction_ShouldReturnOk()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);
        var comment = await CreateTestCommentAsync(user.Id, review.Id);
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Create initial upvote
        var initialReaction = new CreateReactionRequest
        {
            CommentId = comment.Id,
            Type = ReactionType.Upvote
        };
        await Client.PostAsJsonAsync("/api/comments/reaction", initialReaction);

        // Change to downvote
        var updatedReaction = new CreateReactionRequest
        {
            CommentId = comment.Id,
            Type = ReactionType.Downvote
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/comments/reaction", updatedReaction);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var reactionDto = JsonSerializer.Deserialize<ReactionDto>(content, JsonOptions);

        reactionDto.Should().NotBeNull();
        reactionDto!.Type.Should().Be(ReactionType.Downvote);
        reactionDto.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserReaction_WithExistingReaction_ShouldReturnReaction()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);
        var comment = await CreateTestCommentAsync(user.Id, review.Id);
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Create reaction first
        var reactionRequest = new CreateReactionRequest
        {
            CommentId = comment.Id,
            Type = ReactionType.Upvote
        };
        await Client.PostAsJsonAsync("/api/comments/reaction", reactionRequest);

        // Act
        var response = await Client.GetAsync($"/api/comments/reaction/comment/{comment.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var reactionDto = JsonSerializer.Deserialize<ReactionDto>(content, JsonOptions);

        reactionDto.Should().NotBeNull();
        reactionDto!.Type.Should().Be(ReactionType.Upvote);
    }

    [Fact]
    public async Task GetUserReaction_WithoutReaction_ShouldReturnNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);
        var comment = await CreateTestCommentAsync(user.Id, review.Id);
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.GetAsync($"/api/comments/reaction/comment/{comment.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCommentReactionStats_ShouldReturnStats()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user1 = await CreateTestUserAsync("user1@example.com", "user1");
        var user2 = await CreateTestUserAsync("user2@example.com", "user2");
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user1.Id, movie.Id);
        var comment = await CreateTestCommentAsync(user1.Id, review.Id);

        // Create reactions
        DbContext.Reactions.Add(new Reaction
        {
            Id = Guid.NewGuid(),
            UserId = user1.Id,
            CommentId = comment.Id,
            Type = ReactionType.Upvote,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        DbContext.Reactions.Add(new Reaction
        {
            Id = Guid.NewGuid(),
            UserId = user2.Id,
            CommentId = comment.Id,
            Type = ReactionType.Downvote,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await DbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/comments/reaction/comment/{comment.Id}/stats");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        
        // If the response is not OK, let's see what the error is
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception($"Expected OK but got {response.StatusCode}. Response: {content}");
        }
        
        var jsonDoc = JsonDocument.Parse(content);

        jsonDoc.RootElement.GetProperty("commentId").GetGuid().Should().Be(comment.Id);
        jsonDoc.RootElement.GetProperty("upvotesCount").GetInt32().Should().Be(1);
        jsonDoc.RootElement.GetProperty("downvotesCount").GetInt32().Should().Be(1);
        jsonDoc.RootElement.GetProperty("totalReactions").GetInt32().Should().Be(2);
    }

    [Fact]
    public async Task DeleteReaction_WithExistingReaction_ShouldDeleteReaction()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        var movie = await CreateTestMovieAsync();
        var review = await CreateTestReviewAsync(user.Id, movie.Id);
        var comment = await CreateTestCommentAsync(user.Id, review.Id);
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Create reaction first
        var reactionRequest = new CreateReactionRequest
        {
            CommentId = comment.Id,
            Type = ReactionType.Upvote
        };
        await Client.PostAsJsonAsync("/api/comments/reaction", reactionRequest);

        // Act
        var response = await Client.DeleteAsync($"/api/comments/reaction/comment/{comment.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getReactionResponse = await Client.GetAsync($"/api/comments/reaction/comment/{comment.Id}");
        getReactionResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<Comment> CreateTestCommentAsync(Guid userId, Guid reviewId, string content = "This is a test comment.", Guid? parentCommentId = null)
    {
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ReviewId = reviewId,
            Content = content,
            ParentCommentId = parentCommentId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        DbContext.Comments.Add(comment);
        await DbContext.SaveChangesAsync();
        return comment;
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