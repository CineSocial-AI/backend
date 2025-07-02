using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CineSocial.Infrastructure.Services;
using CineSocial.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CineSocial.Tests.Infrastructure.Services;

public class JwtServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly JwtService _jwtService;
    private readonly Dictionary<string, string> _configValues;

    public JwtServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _configValues = new Dictionary<string, string>
        {
            ["Jwt:SecretKey"] = "ThisIsATestSecretKeyThatIsLongEnoughForHmacSha256Algorithm",
            ["Jwt:Issuer"] = "TestIssuer",
            ["Jwt:Audience"] = "TestAudience",
            ["Jwt:AccessTokenExpirationMinutes"] = "15",
            ["Jwt:RefreshTokenExpirationDays"] = "7"
        };

        _configurationMock.Setup(x => x[It.IsAny<string>()])
            .Returns<string>(key => _configValues.TryGetValue(key, out var value) ? value : null);

        _jwtService = new JwtService(_configurationMock.Object);
    }

    [Fact]
    public void Constructor_MissingSecretKey_ShouldThrowException()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["Jwt:SecretKey"]).Returns((string?)null);

        // Act & Assert
        var action = () => new JwtService(configMock.Object);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("JWT SecretKey not configured");
    }

    [Fact]
    public void Constructor_MissingIssuer_ShouldThrowException()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["Jwt:SecretKey"]).Returns("test-secret-key");
        configMock.Setup(x => x["Jwt:Issuer"]).Returns((string?)null);

        // Act & Assert
        var action = () => new JwtService(configMock.Object);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("JWT Issuer not configured");
    }

    [Fact]
    public void Constructor_MissingAudience_ShouldThrowException()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["Jwt:SecretKey"]).Returns("test-secret-key");
        configMock.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
        configMock.Setup(x => x["Jwt:Audience"]).Returns((string?)null);

        // Act & Assert
        var action = () => new JwtService(configMock.Object);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("JWT Audience not configured");
    }

    [Fact]
    public void GenerateToken_ValidUser_ShouldReturnValidJwtToken()
    {
        // Arrange
        var user = TestHelper.CreateTestUser("test@example.com", "testuser");

        // Act
        var token = _jwtService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        jwtToken.Issuer.Should().Be("TestIssuer");
        jwtToken.Audiences.Should().Contain("TestAudience");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == user.Username);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == user.Email);
        jwtToken.Claims.Should().Contain(c => c.Type == "jti");
        jwtToken.Claims.Should().Contain(c => c.Type == "iat");
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnBase64String()
    {
        // Act
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Assert
        refreshToken.Should().NotBeNullOrEmpty();
        
        // Should be able to convert from base64
        var action = () => Convert.FromBase64String(refreshToken);
        action.Should().NotThrow();
        
        var bytes = Convert.FromBase64String(refreshToken);
        bytes.Should().HaveCount(32);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnDifferentTokensEachTime()
    {
        // Act
        var token1 = _jwtService.GenerateRefreshToken();
        var token2 = _jwtService.GenerateRefreshToken();

        // Assert
        token1.Should().NotBe(token2);
    }

    [Fact]
    public void ValidateToken_ValidToken_ShouldReturnTrue()
    {
        // Arrange
        var user = TestHelper.CreateTestUser("test@example.com", "testuser");
        var token = _jwtService.GenerateToken(user);

        // Act
        var isValid = _jwtService.ValidateToken(token);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateToken_InvalidToken_ShouldReturnFalse()
    {
        // Arrange
        var invalidToken = "invalid.jwt.token";

        // Act
        var isValid = _jwtService.ValidateToken(invalidToken);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateToken_EmptyToken_ShouldReturnFalse()
    {
        // Act
        var isValid = _jwtService.ValidateToken("");

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void GetUserIdFromToken_ValidToken_ShouldReturnUserId()
    {
        // Arrange
        var user = TestHelper.CreateTestUser("test@example.com", "testuser");
        var token = _jwtService.GenerateToken(user);

        // Act
        var userId = _jwtService.GetUserIdFromToken(token);

        // Assert
        userId.Should().Be(user.Id);
    }

    [Fact]
    public void GetUserIdFromToken_InvalidToken_ShouldReturnNull()
    {
        // Arrange
        var invalidToken = "invalid.jwt.token";

        // Act
        var userId = _jwtService.GetUserIdFromToken(invalidToken);

        // Assert
        userId.Should().BeNull();
    }

    [Fact]
    public void GetClaimFromToken_ValidTokenAndClaim_ShouldReturnClaimValue()
    {
        // Arrange
        var user = TestHelper.CreateTestUser("test@example.com", "testuser");
        var token = _jwtService.GenerateToken(user);

        // Act
        var email = _jwtService.GetClaimFromToken(token, ClaimTypes.Email);
        var username = _jwtService.GetClaimFromToken(token, ClaimTypes.Name);

        // Assert
        email.Should().Be(user.Email);
        username.Should().Be(user.Username);
    }

    [Fact]
    public void GetClaimFromToken_ValidTokenNonExistentClaim_ShouldReturnNull()
    {
        // Arrange
        var user = TestHelper.CreateTestUser("test@example.com", "testuser");
        var token = _jwtService.GenerateToken(user);

        // Act
        var claim = _jwtService.GetClaimFromToken(token, "non-existent-claim");

        // Assert
        claim.Should().BeNull();
    }

    [Fact]
    public void GetClaimFromToken_InvalidToken_ShouldReturnNull()
    {
        // Arrange
        var invalidToken = "invalid.jwt.token";

        // Act
        var claim = _jwtService.GetClaimFromToken(invalidToken, ClaimTypes.Email);

        // Assert
        claim.Should().BeNull();
    }

    [Fact]
    public void GetTokenExpiryMinutes_ShouldReturnConfiguredValue()
    {
        // Act
        var expiryMinutes = _jwtService.GetTokenExpiryMinutes();

        // Assert
        expiryMinutes.Should().Be(15);
    }

    [Fact]
    public void Constructor_InvalidExpirationMinutes_ShouldUseDefaultValue()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        var configValues = new Dictionary<string, string>
        {
            ["Jwt:SecretKey"] = "ThisIsATestSecretKeyThatIsLongEnoughForHmacSha256Algorithm",
            ["Jwt:Issuer"] = "TestIssuer",
            ["Jwt:Audience"] = "TestAudience",
            ["Jwt:AccessTokenExpirationMinutes"] = "invalid-number",
            ["Jwt:RefreshTokenExpirationDays"] = "7"
        };

        configMock.Setup(x => x[It.IsAny<string>()])
            .Returns<string>(key => configValues.TryGetValue(key, out var value) ? value : null);

        // Act
        var jwtService = new JwtService(configMock.Object);
        var expiryMinutes = jwtService.GetTokenExpiryMinutes();

        // Assert
        expiryMinutes.Should().Be(15); // Default value
    }
}