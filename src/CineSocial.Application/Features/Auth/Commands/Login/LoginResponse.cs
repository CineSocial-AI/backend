namespace CineSocial.Application.Features.Auth.Commands.Login;

public record LoginResponse(string Token, int UserId, string Username, string Email, string Role);
