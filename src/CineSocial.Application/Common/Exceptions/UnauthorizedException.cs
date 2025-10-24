namespace CineSocial.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when authentication fails
/// </summary>
public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message = "Invalid credentials.")
        : base(message, "AUTH_001", StatusCodes.Status401Unauthorized)
    {
    }

    public UnauthorizedException(string message, Exception innerException)
        : base(message, "AUTH_001", StatusCodes.Status401Unauthorized, innerException)
    {
    }
}
