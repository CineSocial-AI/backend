namespace CineSocial.Application.Features.Auth.Commands.Register;

public record RegisterResponse(int UserId, string Username, string Email, string Role);
