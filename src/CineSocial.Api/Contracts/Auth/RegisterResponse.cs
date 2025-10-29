namespace CineSocial.Api.Contracts.Auth;

public record RegisterResponse(
    int UserId,
    string Username,
    string Email,
    string Role
);
