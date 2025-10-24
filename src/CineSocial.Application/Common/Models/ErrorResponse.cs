namespace CineSocial.Application.Common.Models;

/// <summary>
/// Structured error response model
/// </summary>
public class ErrorResponse
{
    public string ErrorCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? StackTrace { get; set; }
    public IDictionary<string, string[]>? ValidationErrors { get; set; }
}
