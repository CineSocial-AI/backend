namespace CineSocial.Api.GraphQL.Payloads;

public class RegisterPayload
{
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public UserError? Error { get; set; }
}
