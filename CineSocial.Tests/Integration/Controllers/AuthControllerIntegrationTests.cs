using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CineSocial.Api.DTOs;
using CineSocial.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CineSocial.Tests.Integration.Controllers;

public class AuthControllerIntegrationTests : IntegrationTestBase
{
    public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_ValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestUserAsync();

        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, JsonOptions);

        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
        authResponse.RefreshToken.Should().NotBeNullOrEmpty();
        authResponse.User.Should().NotBeNull();
        authResponse.User.Email.Should().Be(loginRequest.Email);
        authResponse.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestUserAsync();

        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "wrongpassword"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_NonExistentUser_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();

        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_InactiveUser_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        user.IsActive = false;
        await DbContext.SaveChangesAsync();

        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("", "password123", "Email")]
    [InlineData("invalid-email", "password123", "Email")]
    [InlineData("test@example.com", "", "Password")]
    [InlineData("test@example.com", "123", "Password")]
    public async Task Login_InvalidData_ShouldReturnBadRequest(string email, string password, string expectedErrorField)
    {
        // Arrange
        await ClearDatabaseAsync();

        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_ValidData_ShouldReturnCreated()
    {
        // Arrange
        await ClearDatabaseAsync();

        var registerRequest = new RegisterRequest
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "NewPassword123!",
            FirstName = "New",
            LastName = "User",
            Bio = "Test bio"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, JsonOptions);

        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
        authResponse.RefreshToken.Should().NotBeNullOrEmpty();
        authResponse.User.Should().NotBeNull();
        authResponse.User.Email.Should().Be(registerRequest.Email);
        authResponse.User.Username.Should().Be(registerRequest.Username);
        authResponse.User.FirstName.Should().Be(registerRequest.FirstName);
        authResponse.User.LastName.Should().Be(registerRequest.LastName);
        authResponse.User.Bio.Should().Be(registerRequest.Bio);

        // Verify user was created in database
        var user = await DbContext.Users.FindAsync(authResponse.User.Id);
        user.Should().NotBeNull();
        user!.Email.Should().Be(registerRequest.Email);
    }

    [Fact]
    public async Task Register_ExistingEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestUserAsync("existing@example.com");

        var registerRequest = new RegisterRequest
        {
            Username = "newuser",
            Email = "existing@example.com",
            Password = "NewPassword123!",
            FirstName = "New",
            LastName = "User"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_ExistingUsername_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestUserAsync("test@example.com", "existinguser");

        var registerRequest = new RegisterRequest
        {
            Username = "existinguser",
            Email = "new@example.com",
            Password = "NewPassword123!",
            FirstName = "New",
            LastName = "User"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("", "test@example.com", "Password123!", "Test", "User")]
    [InlineData("ab", "test@example.com", "Password123!", "Test", "User")]
    [InlineData("testuser", "", "Password123!", "Test", "User")]
    [InlineData("testuser", "invalid-email", "Password123!", "Test", "User")]
    [InlineData("testuser", "test@example.com", "", "Test", "User")]
    [InlineData("testuser", "test@example.com", "123", "Test", "User")]
    [InlineData("testuser", "test@example.com", "Password123!", "", "User")]
    [InlineData("testuser", "test@example.com", "Password123!", "Test", "")]
    public async Task Register_InvalidData_ShouldReturnBadRequest(string username, string email, string password, string firstName, string lastName)
    {
        // Arrange
        await ClearDatabaseAsync();

        var registerRequest = new RegisterRequest
        {
            Username = username,
            Email = email,
            Password = password,
            FirstName = firstName,
            LastName = lastName
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProfile_WithValidToken_ShouldReturnUserProfile()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestUserAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.GetAsync("/api/auth/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var userDto = JsonSerializer.Deserialize<UserDto>(content, JsonOptions);

        userDto.Should().NotBeNull();
        userDto!.Email.Should().Be("test@example.com");
        userDto.Username.Should().Be("testuser");
        userDto.FirstName.Should().Be("Test");
        userDto.LastName.Should().Be("User");
    }

    [Fact]
    public async Task GetProfile_WithoutToken_ShouldReturnUnauthorized()
    {
        // Arrange
        await ClearDatabaseAsync();
        ClearAuthorizationHeader();

        // Act
        var response = await Client.GetAsync("/api/auth/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProfile_WithInvalidToken_ShouldReturnUnauthorized()
    {
        // Arrange
        await ClearDatabaseAsync();
        SetAuthorizationHeader("invalid-token");

        // Act
        var response = await Client.GetAsync("/api/auth/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProfile_WithExpiredToken_ShouldReturnUnauthorized()
    {
        // Arrange
        await ClearDatabaseAsync();
        var expiredToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJleHAiOjE1MTYyMzkwMjJ9.invalid";
        SetAuthorizationHeader(expiredToken);

        // Act
        var response = await Client.GetAsync("/api/auth/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_WithValidRefreshToken_ShouldReturnNewTokens()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestUserAsync();
        var authResponse = await LoginUserAsync();

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/refresh", authResponse.RefreshToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var newAuthResponse = JsonSerializer.Deserialize<AuthResponse>(content, JsonOptions);

        newAuthResponse.Should().NotBeNull();
        newAuthResponse!.Token.Should().NotBeNullOrEmpty();
        newAuthResponse.RefreshToken.Should().NotBeNullOrEmpty();
        newAuthResponse.Token.Should().NotBe(authResponse.Token);
        newAuthResponse.RefreshToken.Should().NotBe(authResponse.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidRefreshToken_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/refresh", "invalid-refresh-token");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshToken_WithExpiredRefreshToken_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();
        var user = await CreateTestUserAsync();
        
        // Set expired refresh token
        user.RefreshToken = "expired-token";
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(-1);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/refresh", "expired-token");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Logout_WithValidToken_ShouldReturnOk()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestUserAsync();
        var authResponse = await LoginUserAsync();
        SetAuthorizationHeader(authResponse.Token);

        // Act
        var response = await Client.PostAsync("/api/auth/logout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Logout_WithoutToken_ShouldReturnUnauthorized()
    {
        // Arrange
        await ClearDatabaseAsync();
        ClearAuthorizationHeader();

        // Act
        var response = await Client.PostAsync("/api/auth/logout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AuthFlow_CompleteFlow_ShouldWorkCorrectly()
    {
        // Arrange
        await ClearDatabaseAsync();

        var registerRequest = new RegisterRequest
        {
            Username = "flowuser",
            Email = "flow@example.com",
            Password = "FlowPassword123!",
            FirstName = "Flow",
            LastName = "User"
        };

        // Act & Assert

        // 1. Register
        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var registerContent = await registerResponse.Content.ReadAsStringAsync();
        var registerAuthResponse = JsonSerializer.Deserialize<AuthResponse>(registerContent, JsonOptions);

        // 2. Get Profile with registration token
        SetAuthorizationHeader(registerAuthResponse!.Token);
        var profileResponse = await Client.GetAsync("/api/auth/profile");
        profileResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 3. Logout
        var logoutResponse = await Client.PostAsync("/api/auth/logout", null);
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 4. Login with credentials
        var loginRequest = new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        };

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginAuthResponse = JsonSerializer.Deserialize<AuthResponse>(loginContent, JsonOptions);

        // 5. Refresh token
        var refreshResponse = await Client.PostAsJsonAsync("/api/auth/refresh", loginAuthResponse!.RefreshToken);
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var refreshContent = await refreshResponse.Content.ReadAsStringAsync();
        var refreshAuthResponse = JsonSerializer.Deserialize<AuthResponse>(refreshContent, JsonOptions);

        // 6. Use refreshed token to access profile
        SetAuthorizationHeader(refreshAuthResponse!.Token);
        var finalProfileResponse = await Client.GetAsync("/api/auth/profile");
        finalProfileResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}