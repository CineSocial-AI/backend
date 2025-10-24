namespace CineSocial.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when user doesn''t have permission to perform an action
/// </summary>
public class ForbiddenException : BaseException
{
    public ForbiddenException(string message = "You don''t have permission to perform this action.")
        : base(message, "AUTH_002", StatusCodes.Status403Forbidden)
    {
    }

    public ForbiddenException(string message, Exception innerException)
        : base(message, "AUTH_002", StatusCodes.Status403Forbidden, innerException)
    {
    }
}
