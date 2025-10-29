namespace CineSocial.Api.Contracts.Auth;

public record LoginResponse(
    string Token,
    int UserId,
    string Username,
    string Email,
    string Role
);
