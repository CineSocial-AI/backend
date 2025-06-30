using Xunit;
using FluentAssertions;

namespace CineSocial.Tests;

public class BasicTest
{
    [Fact]
    public void TestFramework_ShouldWork()
    {
        // Arrange
        var value = 42;

        // Act
        var result = value * 2;

        // Assert
        result.Should().Be(84);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(0, 0, 0)]
    [InlineData(-1, 1, 0)]
    public void Addition_ShouldWork(int a, int b, int expected)
    {
        // Act
        var result = a + b;

        // Assert
        result.Should().Be(expected);
    }
}