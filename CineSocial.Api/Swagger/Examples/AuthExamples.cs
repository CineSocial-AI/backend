using CineSocial.Api.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace CineSocial.Api.Swagger.Examples;

public class LoginRequestExample : IExamplesProvider<LoginRequest>
{
    public LoginRequest GetExamples()
    {
        return new LoginRequest
        {
            Email = "cinemafan01@example.com",
            Password = "Password123!"
        };
    }
}

public class RegisterRequestExample : IExamplesProvider<RegisterRequest>
{
    public RegisterRequest GetExamples()
    {
        return new RegisterRequest
        {
            Username = "moviefan2024",
            Email = "moviefan2024@example.com",
            Password = "SecurePassword123!",
            FirstName = "Ahmet",
            LastName = "Yılmaz",
            Bio = "Film tutkunu ve eleştirmeni. Özellikle aksiyon ve bilim kurgu filmleri seviyorum."
        };
    }
}

public class AuthResponseExample : IExamplesProvider<AuthResponse>
{
    public AuthResponse GetExamples()
    {
        return new AuthResponse
        {
            Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
            RefreshToken = "550e8400-e29b-41d4-a716-446655440000",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = new UserDto
            {
                Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
                Username = "moviefan2024",
                Email = "moviefan2024@example.com",
                FirstName = "Ahmet",
                LastName = "Yılmaz",
                Bio = "Film tutkunu ve eleştirmeni. Özellikle aksiyon ve bilim kurgu filmleri seviyorum.",
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            }
        };
    }
}

public class UserDtoExample : IExamplesProvider<UserDto>
{
    public UserDto GetExamples()
    {
        return new UserDto
        {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            Username = "cinemafan01",
            Email = "cinemafan01@example.com",
            FirstName = "Elif",
            LastName = "Kaya",
            Bio = "Sinema yazarı ve film eleştirmeni. Her türden filme açığım.",
            CreatedAt = DateTime.UtcNow.AddDays(-60)
        };
    }
}