using CineSocial.Core.Localization;
using FluentValidation;
using Serilog;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace CineSocial.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public GlobalExceptionMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionMiddleware> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred. RequestPath: {RequestPath}, Method: {Method}, User: {User}",
                context.Request.Path, context.Request.Method, context.User?.Identity?.Name ?? "Anonymous");

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new ErrorResponse();

        // Create a scope to access scoped services
        using var scope = _serviceScopeFactory.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();

        switch (exception)
        {
            case ValidationException validationEx:
                response.Title = localizationService.GetString("Error.Validation.Title");
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = localizationService.GetString("Error.Validation.Message");
                response.Errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList();
                
                _logger.LogWarning("Validation error occurred: {ValidationErrors}", 
                    string.Join(", ", response.Errors));
                break;

            case UnauthorizedAccessException:
                response.Title = localizationService.GetString("Error.Unauthorized.Title");
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = localizationService.GetString("Error.Unauthorized.Message");
                
                _logger.LogWarning("Unauthorized access attempt: {User}", 
                    context.User?.Identity?.Name ?? "Anonymous");
                break;

            case KeyNotFoundException:
                response.Title = localizationService.GetString("Error.NotFound.Title");
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = exception.Message;
                
                _logger.LogInformation("Resource not found: {Message}", exception.Message);
                break;

            case ArgumentException argEx:
                response.Title = localizationService.GetString("Error.BadRequest.Title");
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = argEx.Message;
                
                _logger.LogWarning("Bad request: {Message}", argEx.Message);
                break;

            case InvalidOperationException invalidOpEx:
                response.Title = localizationService.GetString("Error.Conflict.Title");
                response.StatusCode = (int)HttpStatusCode.Conflict;
                response.Message = invalidOpEx.Message;
                
                _logger.LogWarning("Invalid operation: {Message}", invalidOpEx.Message);
                break;

            case TimeoutException:
                response.Title = localizationService.GetString("Error.Timeout.Title");
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                response.Message = localizationService.GetString("Error.Timeout.Message");
                
                _logger.LogError("Request timeout occurred");
                break;

            default:
                response.Title = localizationService.GetString("Error.Internal.Title");
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = localizationService.GetString("Error.Internal.Message");
                
                // Log full exception details for internal server errors
                _logger.LogError(exception, "Internal server error occurred: {ExceptionType}", 
                    exception.GetType().Name);
                break;
        }

        context.Response.StatusCode = response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

public class ErrorResponse
{
    public string Title { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string>? Errors { get; set; }
    public string TraceId { get; set; } = Activity.Current?.Id ?? string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}