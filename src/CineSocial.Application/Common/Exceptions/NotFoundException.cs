namespace CineSocial.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class NotFoundException : BaseException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with id ''{key}'' was not found.", "NOT_FOUND_001", StatusCodes.Status404NotFound)
    {
    }

    public NotFoundException(string message)
        : base(message, "NOT_FOUND_001", StatusCodes.Status404NotFound)
    {
    }
}
