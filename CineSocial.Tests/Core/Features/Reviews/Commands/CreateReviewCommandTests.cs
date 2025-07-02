using CineSocial.Core.Features.Reviews.Commands;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using CineSocial.Tests.Helpers;
using FluentAssertions;
using Moq;

namespace CineSocial.Tests.Core.Features.Reviews.Commands;

public class CreateReviewCommandTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Movie>> _movieRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRepository<Review>> _reviewRepositoryMock;
    private readonly CreateReviewCommandHandler _handler;

    public CreateReviewCommandTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _movieRepositoryMock = new Mock<IRepository<Movie>>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _reviewRepositoryMock = new Mock<IRepository<Review>>();

        _unitOfWorkMock.Setup(x => x.Movies).Returns(_movieRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.Users).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.Reviews).Returns(_reviewRepositoryMock.Object);

        _handler = new CreateReviewCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateReviewSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        var user = TestHelper.CreateTestUser("test@example.com", "testuser", userId);
        var movie = TestHelper.CreateTestMovie("Test Movie", movieId);
        
        var command = new CreateReviewCommand(
            userId,
            movieId,
            "Great Movie!",
            "This is an excellent movie with great acting and storyline.",
            false);

        _movieRepositoryMock.Setup(x => x.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _reviewRepositoryMock.Setup(x => x.FirstOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Review, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((Review?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.UserId.Should().Be(userId);
        result.Data.MovieId.Should().Be(movieId);
        result.Data.Title.Should().Be(command.Title);
        result.Data.Content.Should().Be(command.Content);
        result.Data.ContainsSpoilers.Should().Be(command.ContainsSpoilers);
        result.Data.LikesCount.Should().Be(0);
        result.Data.UserFullName.Should().Be($"{user.FirstName} {user.LastName}");
        result.Data.UserUsername.Should().Be(user.Username);

        _reviewRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()), Times.Once);
        // Verify SaveChangesAsync was called
    }

    [Fact]
    public async Task Handle_MovieNotFound_ShouldReturnFailureResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        
        var command = new CreateReviewCommand(
            userId,
            movieId,
            "Great Movie!",
            "This is an excellent movie.",
            false);

        _movieRepositoryMock.Setup(x => x.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Film bulunamadı.");

        _reviewRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()), Times.Never);
        // Verify SaveChangesAsync was not called
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldReturnFailureResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        var movie = TestHelper.CreateTestMovie("Test Movie", movieId);
        
        var command = new CreateReviewCommand(
            userId,
            movieId,
            "Great Movie!",
            "This is an excellent movie.",
            false);

        _movieRepositoryMock.Setup(x => x.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Kullanıcı bulunamadı.");

        _reviewRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()), Times.Never);
        // Verify SaveChangesAsync was not called
    }

    [Fact]
    public async Task Handle_UserAlreadyReviewed_ShouldReturnFailureResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        var user = TestHelper.CreateTestUser("test@example.com", "testuser", userId);
        var movie = TestHelper.CreateTestMovie("Test Movie", movieId);
        var existingReview = TestHelper.CreateTestReview(userId, movieId);
        
        var command = new CreateReviewCommand(
            userId,
            movieId,
            "Great Movie!",
            "This is an excellent movie.",
            false);

        _movieRepositoryMock.Setup(x => x.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _reviewRepositoryMock.Setup(x => x.FirstOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Review, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReview);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Bu film için zaten bir değerlendirme yazmışsınız.");

        _reviewRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()), Times.Never);
        // Verify SaveChangesAsync was not called
    }

    [Fact]
    public async Task Handle_ReviewWithSpoilers_ShouldCreateReviewWithSpoilersFlag()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        var user = TestHelper.CreateTestUser("test@example.com", "testuser", userId);
        var movie = TestHelper.CreateTestMovie("Test Movie", movieId);
        
        var command = new CreateReviewCommand(
            userId,
            movieId,
            "Great Movie!",
            "This movie has a great twist ending where the hero dies.",
            true); // Contains spoilers

        _movieRepositoryMock.Setup(x => x.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _reviewRepositoryMock.Setup(x => x.FirstOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Review, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((Review?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.ContainsSpoilers.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ReviewCreatedAt_ShouldBeSetToCurrentTime()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        var user = TestHelper.CreateTestUser("test@example.com", "testuser", userId);
        var movie = TestHelper.CreateTestMovie("Test Movie", movieId);
        var beforeCreation = DateTime.UtcNow;
        
        var command = new CreateReviewCommand(
            userId,
            movieId,
            "Great Movie!",
            "This is an excellent movie.",
            false);

        _movieRepositoryMock.Setup(x => x.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _reviewRepositoryMock.Setup(x => x.FirstOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Review, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((Review?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        var afterCreation = DateTime.UtcNow;

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.CreatedAt.Should().BeOnOrAfter(beforeCreation);
        result.Data.CreatedAt.Should().BeOnOrBefore(afterCreation);
        result.Data.UpdatedAt.Should().BeNull();
    }
}