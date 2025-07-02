namespace CineSocial.Core.Shared;

public sealed record Error(string Code, string Message, string? Details = null)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static implicit operator string(Error error) => error.Code;

    public static implicit operator Error(string code) => new(code, code);

    public override string ToString() => Code;
}