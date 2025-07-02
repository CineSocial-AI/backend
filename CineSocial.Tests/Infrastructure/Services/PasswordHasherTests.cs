using CineSocial.Infrastructure.Services;
using FluentAssertions;

namespace CineSocial.Tests.Infrastructure.Services;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher;

    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Fact]
    public void HashPassword_ValidPassword_ShouldReturnHashedPassword()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hashedPassword = _passwordHasher.HashPassword(password);

        // Assert
        hashedPassword.Should().NotBeNullOrEmpty();
        hashedPassword.Should().NotBe(password);
        
        // Should be base64 encoded
        var action = () => Convert.FromBase64String(hashedPassword);
        action.Should().NotThrow();
        
        // Should have correct length (32 bytes salt + 32 bytes hash = 64 bytes)
        var bytes = Convert.FromBase64String(hashedPassword);
        bytes.Should().HaveCount(64);
    }

    [Fact]
    public void HashPassword_SamePasswordTwice_ShouldReturnDifferentHashes()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void HashPassword_NullOrEmptyPassword_ShouldThrowArgumentException(string? password)
    {
        // Act & Assert
        var action = () => _passwordHasher.HashPassword(password!);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Password cannot be null or empty*");
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "TestPassword123!";
        var hashedPassword = _passwordHasher.HashPassword(password);

        // Act
        var isValid = _passwordHasher.VerifyPassword(password, hashedPassword);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_IncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var wrongPassword = "WrongPassword123!";
        var hashedPassword = _passwordHasher.HashPassword(password);

        // Act
        var isValid = _passwordHasher.VerifyPassword(wrongPassword, hashedPassword);

        // Assert
        isValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void VerifyPassword_NullOrEmptyPassword_ShouldReturnFalse(string? password)
    {
        // Arrange
        var hashedPassword = _passwordHasher.HashPassword("TestPassword123!");

        // Act
        var isValid = _passwordHasher.VerifyPassword(password!, hashedPassword);

        // Assert
        isValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void VerifyPassword_NullOrEmptyHashedPassword_ShouldReturnFalse(string? hashedPassword)
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var isValid = _passwordHasher.VerifyPassword(password, hashedPassword!);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_InvalidHashedPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var invalidHashedPassword = "InvalidHashedPassword";

        // Act
        var isValid = _passwordHasher.VerifyPassword(password, invalidHashedPassword);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_HashedPasswordWithWrongLength_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var shortHash = Convert.ToBase64String(new byte[32]); // Only 32 bytes instead of 64

        // Act
        var isValid = _passwordHasher.VerifyPassword(password, shortHash);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_CaseSensitive_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var wrongCasePassword = "testpassword123!";
        var hashedPassword = _passwordHasher.HashPassword(password);

        // Act
        var isValid = _passwordHasher.VerifyPassword(wrongCasePassword, hashedPassword);

        // Assert
        isValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("simple")]
    [InlineData("ComplexPassword123!@#")]
    [InlineData("UPPERCASE")]
    [InlineData("lowercase")]
    [InlineData("1234567890")]
    [InlineData("!@#$%^&*()")]
    [InlineData("With Spaces")]
    [InlineData("УникодПароль123")]
    public void HashAndVerifyPassword_VariousPasswords_ShouldWorkCorrectly(string password)
    {
        // Act
        var hashedPassword = _passwordHasher.HashPassword(password);
        var isValid = _passwordHasher.VerifyPassword(password, hashedPassword);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void HashPassword_VeryLongPassword_ShouldWork()
    {
        // Arrange
        var longPassword = new string('a', 1000);

        // Act
        var hashedPassword = _passwordHasher.HashPassword(longPassword);
        var isValid = _passwordHasher.VerifyPassword(longPassword, hashedPassword);

        // Assert
        hashedPassword.Should().NotBeNullOrEmpty();
        isValid.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_CorruptedHashInMiddle_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var hashedPassword = _passwordHasher.HashPassword(password);
        
        // Corrupt the hash by changing one character in the middle
        var corruptedHash = hashedPassword.ToCharArray();
        corruptedHash[hashedPassword.Length / 2] = corruptedHash[hashedPassword.Length / 2] == 'A' ? 'B' : 'A';
        var corruptedHashString = new string(corruptedHash);

        // Act
        var isValid = _passwordHasher.VerifyPassword(password, corruptedHashString);

        // Assert
        isValid.Should().BeFalse();
    }
}