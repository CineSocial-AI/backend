using Serilog;
using Serilog.Context;
using System.Diagnostics;
using System.Text;

namespace CineSocial.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = Guid.NewGuid().ToString();
        
        // Add correlation ID to response headers
        context.Response.Headers.Add("X-Correlation-ID", correlationId);
        
        // Enrich logs with correlation ID
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("RequestId", Activity.Current?.Id))
        using (LogContext.PushProperty("UserAgent", context.Request.Headers.UserAgent.ToString()))
        using (LogContext.PushProperty("RemoteIP", GetClientIpAddress(context)))
        {
            var requestInfo = await LogRequest(context);
            
            // Store original response body stream
            var originalResponseBodyStream = context.Response.Body;
            
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;
            
            try
            {
                await _next(context);
                
                stopwatch.Stop();
                
                await LogResponse(context, requestInfo, stopwatch.ElapsedMilliseconds);
                
                // Copy response back to original stream
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                await responseBodyStream.CopyToAsync(originalResponseBodyStream);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                _logger.LogError(ex, 
                    "Request failed: {Method} {Path} - Duration: {Duration}ms - User: {User}",
                    requestInfo.Method, requestInfo.Path, stopwatch.ElapsedMilliseconds,
                    context.User?.Identity?.Name ?? "Anonymous");
                    
                throw;
            }
            finally
            {
                context.Response.Body = originalResponseBodyStream;
            }
        }
    }

    private async Task<RequestInfo> LogRequest(HttpContext context)
    {
        var request = context.Request;
        var requestInfo = new RequestInfo
        {
            Method = request.Method,
            Path = request.Path,
            QueryString = request.QueryString.ToString(),
            ContentType = request.ContentType,
            ContentLength = request.ContentLength,
            UserName = context.User?.Identity?.Name ?? "Anonymous",
            UserAgent = request.Headers.UserAgent.ToString(),
            RemoteIP = GetClientIpAddress(context)
        };

        // Log request body for POST/PUT requests (be careful with sensitive data)
        if (ShouldLogRequestBody(request))
        {
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength ?? 0)];
            await request.Body.ReadAsync(buffer);
            requestInfo.Body = Encoding.UTF8.GetString(buffer);
            request.Body.Position = 0;
        }

        _logger.LogInformation(
            "Request started: {Method} {Path}{QueryString} - User: {User} - IP: {RemoteIP} - ContentType: {ContentType}",
            requestInfo.Method, requestInfo.Path, requestInfo.QueryString, 
            requestInfo.UserName, requestInfo.RemoteIP, requestInfo.ContentType);

        if (!string.IsNullOrEmpty(requestInfo.Body))
        {
            _logger.LogDebug("Request body: {RequestBody}", SanitizeRequestBody(requestInfo.Body));
        }

        return requestInfo;
    }

    private async Task LogResponse(HttpContext context, RequestInfo requestInfo, long durationMs)
    {
        var response = context.Response;
        
        // Read response body
        string responseBody = string.Empty;
        if (ShouldLogResponseBody(response))
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(response.Body, leaveOpen: true);
            responseBody = await reader.ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
        }

        var logLevel = GetLogLevelForStatusCode(response.StatusCode);
        
        _logger.Log(logLevel,
            "Request completed: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms - User: {User} - Size: {ResponseSize}",
            requestInfo.Method, requestInfo.Path, response.StatusCode, durationMs,
            requestInfo.UserName, response.Body.Length);

        if (!string.IsNullOrEmpty(responseBody) && response.StatusCode >= 400)
        {
            _logger.LogWarning("Error response body: {ResponseBody}", SanitizeResponseBody(responseBody));
        }

        // Log slow requests
        if (durationMs > 5000) // 5 seconds
        {
            _logger.LogWarning(
                "Slow request detected: {Method} {Path} - Duration: {Duration}ms - User: {User}",
                requestInfo.Method, requestInfo.Path, durationMs, requestInfo.UserName);
        }
    }

    private bool ShouldLogRequestBody(HttpRequest request)
    {
        if (request.ContentLength == null || request.ContentLength == 0) return false;
        if (request.ContentLength > 1024 * 1024) return false; // Don't log bodies > 1MB
        
        var contentType = request.ContentType?.ToLower() ?? "";
        return contentType.Contains("application/json") || 
               contentType.Contains("application/xml") ||
               contentType.Contains("text/");
    }

    private bool ShouldLogResponseBody(HttpResponse response)
    {
        if (response.Body.Length > 1024 * 1024) return false; // Don't log bodies > 1MB
        
        var contentType = response.ContentType?.ToLower() ?? "";
        return (contentType.Contains("application/json") || 
                contentType.Contains("application/xml") ||
                contentType.Contains("text/")) &&
                response.StatusCode >= 400; // Only log error responses
    }

    private string SanitizeRequestBody(string body)
    {
        // Remove sensitive information from logs
        if (string.IsNullOrEmpty(body)) return body;
        
        try
        {
            // Remove password fields from JSON
            var sensitiveFields = new[] { "password", "token", "secret", "key", "authorization" };
            foreach (var field in sensitiveFields)
            {
                var pattern = $"\"{field}\"\\s*:\\s*\"[^\"]*\"";
                body = System.Text.RegularExpressions.Regex.Replace(body, pattern, 
                    $"\"{field}\": \"***REDACTED***\"", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
        }
        catch
        {
            return "***SANITIZATION_FAILED***";
        }
        
        return body;
    }

    private string SanitizeResponseBody(string body)
    {
        // Similar sanitization for response bodies
        if (string.IsNullOrEmpty(body)) return body;
        
        try
        {
            var sensitiveFields = new[] { "token", "refreshToken", "secret", "key" };
            foreach (var field in sensitiveFields)
            {
                var pattern = $"\"{field}\"\\s*:\\s*\"[^\"]*\"";
                body = System.Text.RegularExpressions.Regex.Replace(body, pattern, 
                    $"\"{field}\": \"***REDACTED***\"", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
        }
        catch
        {
            return "***SANITIZATION_FAILED***";
        }
        
        return body;
    }

    private LogLevel GetLogLevelForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            >= 500 => LogLevel.Error,
            >= 400 => LogLevel.Warning,
            >= 300 => LogLevel.Information,
            _ => LogLevel.Information
        };
    }

    private string GetClientIpAddress(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        
        // Check for forwarded IP in headers (for load balancers/proxies)
        if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            var forwardedIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedIp))
            {
                ipAddress = forwardedIp.Split(',')[0].Trim();
            }
        }
        else if (context.Request.Headers.ContainsKey("X-Real-IP"))
        {
            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                ipAddress = realIp;
            }
        }
        
        return ipAddress ?? "Unknown";
    }
}

public class RequestInfo
{
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string QueryString { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long? ContentLength { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string RemoteIP { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}