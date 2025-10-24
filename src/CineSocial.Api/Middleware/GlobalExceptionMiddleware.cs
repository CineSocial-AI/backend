using System.Net;
using System.Text.Json;
using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Models;

namespace CineSocial.Api.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        ErrorResponse errorResponse;
        int statusCode;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = validationException.StatusCode;
                errorResponse = new ErrorResponse
                {
                    ErrorCode = validationException.ErrorCode,
                    Message = validationException.Message,
                    StatusCode = statusCode,
                    ValidationErrors = validationException.Errors
                };
                _logger.LogWarning(exception, "Validation error: {Message}", validationException.Message);
                break;

            case BaseException baseException:
                statusCode = baseException.StatusCode;
                errorResponse = new ErrorResponse
                {
                    ErrorCode = baseException.ErrorCode,
                    Message = baseException.Message,
                    StatusCode = statusCode
                };
                _logger.LogWarning(exception, "Application error: {Message}", baseException.Message);
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse = new ErrorResponse
                {
                    ErrorCode = "SERVER_001",
                    Message = _environment.IsDevelopment() 
                        ? exception.Message 
                        : "An internal server error occurred.",
                    StatusCode = statusCode
                };
                _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
                break;
        }

        // Include stack trace only in development
        if (_environment.IsDevelopment())
        {
            errorResponse.StackTrace = exception.StackTrace;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
