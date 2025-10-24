namespace CineSocial.Application.Common.Models;

/// <summary>
/// Detailed validation error information
/// </summary>
public class ValidationErrorDetail
{
    public string Field { get; set; } = string.Empty;
    public string[] Errors { get; set; } = Array.Empty<string>();
}
