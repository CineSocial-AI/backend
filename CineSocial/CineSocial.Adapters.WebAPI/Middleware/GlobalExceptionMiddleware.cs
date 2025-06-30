using System.Net;
using System.Text.Json;
using CineSocial.Adapters.WebAPI.DTOs.Responses;
using FluentValidation;

namespace CineSocial.Adapters.WebAPI.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = exception switch
        {
            ValidationException validationEx => CreateValidationErrorResponse(validationEx, HttpStatusCode.BadRequest),
            UnauthorizedAccessException => CreateErrorResponse("Yetkisiz erişim", HttpStatusCode.Unauthorized),
            KeyNotFoundException => CreateErrorResponse("Kaynak bulunamadı", HttpStatusCode.NotFound),
            ArgumentException argEx => CreateErrorResponse(argEx.Message, HttpStatusCode.BadRequest),
            InvalidOperationException invalidOpEx => CreateErrorResponse(invalidOpEx.Message, HttpStatusCode.BadRequest),
            _ => CreateErrorResponse("Sunucu hatası oluştu", HttpStatusCode.InternalServerError)
        };

        context.Response.StatusCode = (int)response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(response.ApiResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private static (HttpStatusCode StatusCode, ApiResponse ApiResponse) CreateErrorResponse(
        string message, 
        HttpStatusCode statusCode)
    {
        return (statusCode, ApiResponse.CreateFailure(message));
    }

    private static (HttpStatusCode StatusCode, ApiResponse ApiResponse) CreateValidationErrorResponse(
        ValidationException validationException, 
        HttpStatusCode statusCode)
    {
        var errors = validationException.Errors.Select(e => e.ErrorMessage).ToList();
        return (statusCode, ApiResponse.CreateFailure(errors));
    }
}