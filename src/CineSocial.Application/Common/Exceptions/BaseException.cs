namespace CineSocial.Application.Common.Exceptions;

/// <summary>
/// Base exception class for all custom application exceptions
/// </summary>
public abstract class BaseException : Exception
{
    public string ErrorCode { get; }
    public int StatusCode { get; }

    protected BaseException(string message, string errorCode, int statusCode)
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    protected BaseException(string message, string errorCode, int statusCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}
