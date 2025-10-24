namespace CineSocial.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when there''s a conflict (e.g., duplicate entry)
/// </summary>
public class ConflictException : BaseException
{
    public ConflictException(string message)
        : base(message, "CONFLICT_001", StatusCodes.Status409Conflict)
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, "CONFLICT_001", StatusCodes.Status409Conflict, innerException)
    {
    }
}
