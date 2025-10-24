namespace CineSocial.Api.GraphQL.Payloads;

public class UserError
{
    public UserError(string code, string message, string? field = null)
    {
        Code = code;
        Message = message;
        Field = field;
    }

    public string Code { get; }
    public string Message { get; }
    public string? Field { get; }
}
