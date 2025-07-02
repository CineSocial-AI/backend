using CineSocial.Core.Features.Auth.Commands;
using CineSocial.Core.Localization;
using CineSocial.Core.Logging;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using CineSocial.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CineSocial.Tests.Core.Features.Auth.Commands;

public class LoginCommandTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly Mock<IAuthenticationLogger> _authLoggerMock;
    private readonly Mock<ILogger<LoginCommandHandler>> _loggerMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtServiceMock = new Mock<IJwtService>();
        _localizationServiceMock = new Mock<ILocalizationService>();
        _authLoggerMock = new Mock<IAuthenticationLogger>();
        _loggerMock = new Mock<ILogger<LoginCommandHandler>>();

        _unitOfWorkMock.Setup(x => x.Users).Returns(_userRepositoryMock.Object);

        _handler = new LoginCommandHandler(
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _localizationServiceMock.Object,
            _authLoggerMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldReturnSuccessResult()
    {
        // Arrange
        var user = TestHelper.CreateTestUser("test@example.com", "testuser");
        var command = new LoginCommand("test@example.com", "password123");
        var expectedToken = "jwt-token";
        var expectedRefreshToken = "refresh-token";

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyPassword(command.Password, user.PasswordHash))
            .Returns(true);
        _jwtServiceMock.Setup(x => x.GenerateToken(user))
            .Returns(expectedToken);
        _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns(expectedRefreshToken);
        _jwtServiceMock.Setup(x => x.GetTokenExpiryMinutes())
            .Returns(15);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().Be(expectedToken);
        result.Data.RefreshToken.Should().Be(expectedRefreshToken);
        result.Data.User.Should().Be(user);

        // Verify SaveChangesAsync was called
        // _authLoggerMock.Verify(x => x.LogSuccessfulLogin(
        //     user.Id.ToString(),
        //     user.Username,
        //     "Unknown"), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldReturnFailureResult()
    {
        // Arrange
        var command = new LoginCommand("nonexistent@example.com", "password123");
        var errorMessage = "User not found";

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync((User?)null);
        _localizationServiceMock.Setup(x => x.GetString("User.NotFound"))
            .Returns(errorMessage);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(errorMessage);

        // _authLoggerMock.Verify(x => x.LogFailedLogin(
        //     command.Email,
        //     "Unknown",
        //     "User not found"), Times.Once);
        // Verify SaveChangesAsync was not called
    }

    [Fact]
    public async Task Handle_InactiveUser_ShouldReturnFailureResult()
    {
        // Arrange
        var user = TestHelper.CreateTestUser("test@example.com", "testuser");
        user.IsActive = false;
        var command = new LoginCommand("test@example.com", "password123");
        var errorMessage = "Account is inactive";

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(user);
        _localizationServiceMock.Setup(x => x.GetString("User.InactiveAccount"))
            .Returns(errorMessage);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(errorMessage);

        // _authLoggerMock.Verify(x => x.LogFailedLogin(
        //     command.Email,
        //     "Unknown",
        //     "Account inactive"), Times.Once);
        // Verify SaveChangesAsync was not called
    }

    [Fact]
    public async Task Handle_InvalidPassword_ShouldReturnFailureResult()
    {
        // Arrange
        var user = TestHelper.CreateTestUser("test@example.com", "testuser");
        var command = new LoginCommand("test@example.com", "wrongpassword");
        var errorMessage = "Invalid credentials";

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyPassword(command.Password, user.PasswordHash))
            .Returns(false);
        _localizationServiceMock.Setup(x => x.GetString("User.InvalidCredentials"))
            .Returns(errorMessage);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(errorMessage);

        // _authLoggerMock.Verify(x => x.LogFailedLogin(
        //     command.Email,
        //     "Unknown",
        //     "Invalid password"), Times.Once);
        // Verify SaveChangesAsync was not called
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldUpdateUserLastLoginAndRefreshToken()
    {
        // Arrange
        var user = TestHelper.CreateTestUser("test@example.com", "testuser");
        var originalLastLogin = user.LastLoginAt;
        var command = new LoginCommand("test@example.com", "password123");
        var expectedRefreshToken = "refresh-token";

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyPassword(command.Password, user.PasswordHash))
            .Returns(true);
        _jwtServiceMock.Setup(x => x.GenerateToken(user))
            .Returns("jwt-token");
        _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns(expectedRefreshToken);
        _jwtServiceMock.Setup(x => x.GetTokenExpiryMinutes())
            .Returns(15);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.LastLoginAt.Should().NotBe(originalLastLogin);
        user.RefreshToken.Should().Be(expectedRefreshToken);
        user.RefreshTokenExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromMinutes(1));
    }
}