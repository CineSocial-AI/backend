namespace CineSocial.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when business logic validation fails
/// </summary>
public class BusinessException : BaseException
{
    public BusinessException(string message, string errorCode = "BUSINESS_001")
        : base(message, errorCode, StatusCodes.Status400BadRequest)
    {
    }

    public BusinessException(string message, string errorCode, Exception innerException)
        : base(message, errorCode, StatusCodes.Status400BadRequest, innerException)
    {
    }
}
