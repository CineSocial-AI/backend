using CineSocial.Domain.Entities;
using CineSocial.Domain.Enums;

namespace CineSocial.Tests.Helpers;

public static class TestHelper
{
    public static User CreateTestUser(string? email = null, string? username = null, Guid? id = null)
    {
        return new User
        {
            Id = id ?? Guid.NewGuid(),
            Email = email ?? "test@example.com",
            Username = username ?? "testuser",
            PasswordHash = "hashedpassword123",
            FirstName = "Test",
            LastName = "User",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Movie CreateTestMovie(string? title = null, Guid? id = null)
    {
        return new Movie
        {
            Id = id ?? Guid.NewGuid(),
            Title = title ?? "Test Movie",
            Overview = "A test movie description",
            ReleaseDate = DateTime.UtcNow.AddDays(-30),
            Runtime = 120,
            VoteAverage = 7.5m,
            VoteCount = 100,
            PosterPath = "/test-poster.jpg",
            BackdropPath = "/test-backdrop.jpg",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Review CreateTestReview(Guid? userId = null, Guid? movieId = null, Guid? id = null)
    {
        return new Review
        {
            Id = id ?? Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            MovieId = movieId ?? Guid.NewGuid(),
            Content = "This is a test review",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Comment CreateTestComment(Guid? userId = null, Guid? reviewId = null, Guid? id = null)
    {
        return new Comment
        {
            Id = id ?? Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            ReviewId = reviewId ?? Guid.NewGuid(),
            Content = "This is a test comment",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Rating CreateTestRating(Guid? userId = null, Guid? movieId = null, double value = 8.0)
    {
        return new Rating
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            MovieId = movieId ?? Guid.NewGuid(),
            Score = (int)value,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Reaction CreateTestReaction(Guid? userId = null, Guid? commentId = null, ReactionType type = ReactionType.Upvote)
    {
        return new Reaction
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            CommentId = commentId ?? Guid.NewGuid(),
            Type = type,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}