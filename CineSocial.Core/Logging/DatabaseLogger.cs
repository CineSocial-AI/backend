using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CineSocial.Core.Logging;

public class DatabaseLogger : IDatabaseLogger
{
    private readonly ILogger<DatabaseLogger> _logger;

    public DatabaseLogger(ILogger<DatabaseLogger> logger)
    {
        _logger = logger;
    }

    public void LogQuery(string operation, string query, TimeSpan duration, string? userId = null)
    {
        var logLevel = GetLogLevelForDuration(duration);
        
        _logger.Log(logLevel,
            "Database Query: {Operation} - Duration: {Duration}ms - User: {UserId}",
            operation, duration.TotalMilliseconds, userId ?? "System");

        if (duration.TotalMilliseconds > 1000) // Log slow queries
        {
            _logger.LogWarning(
                "Slow Query Detected: {Operation} - Duration: {Duration}ms - Query: {Query} - User: {UserId}",
                operation, duration.TotalMilliseconds, SanitizeQuery(query), userId ?? "System");
        }
        else if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(
                "Query Details: {Operation} - Query: {Query} - Duration: {Duration}ms - User: {UserId}",
                operation, SanitizeQuery(query), duration.TotalMilliseconds, userId ?? "System");
        }
    }

    public void LogDatabaseError(string operation, Exception exception, string? query = null, string? userId = null)
    {
        _logger.LogError(exception,
            "Database Error: {Operation} - User: {UserId} - Query: {Query}",
            operation, userId ?? "System", query != null ? SanitizeQuery(query) : "N/A");
    }

    public void LogEntityOperation(string entityName, string operation, object entityId, string? userId = null)
    {
        _logger.LogInformation(
            "Entity Operation: {EntityName}.{Operation} - ID: {EntityId} - User: {UserId}",
            entityName, operation, entityId, userId ?? "System");
    }

    public void LogBulkOperation(string entityName, string operation, int affectedRows, TimeSpan duration, string? userId = null)
    {
        var logLevel = affectedRows > 100 || duration.TotalMilliseconds > 5000 ? LogLevel.Warning : LogLevel.Information;
        
        _logger.Log(logLevel,
            "Bulk Operation: {EntityName}.{Operation} - Affected Rows: {AffectedRows} - Duration: {Duration}ms - User: {UserId}",
            entityName, operation, affectedRows, duration.TotalMilliseconds, userId ?? "System");
    }

    private LogLevel GetLogLevelForDuration(TimeSpan duration)
    {
        return duration.TotalMilliseconds switch
        {
            > 5000 => LogLevel.Error,    // > 5 seconds
            > 1000 => LogLevel.Warning,  // > 1 second
            > 500 => LogLevel.Information, // > 500ms
            _ => LogLevel.Debug
        };
    }

    private string SanitizeQuery(string query)
    {
        if (string.IsNullOrEmpty(query)) return query;
        
        // Remove potential sensitive data from queries
        var sanitized = query;
        
        // Remove password-like patterns
        sanitized = System.Text.RegularExpressions.Regex.Replace(sanitized, 
            @"(password|token|secret|key)\s*=\s*'[^']*'", 
            "$1 = '***REDACTED***'", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
        sanitized = System.Text.RegularExpressions.Regex.Replace(sanitized, 
            @"(password|token|secret|key)\s*=\s*""[^""]*""", 
            "$1 = \"***REDACTED***\"", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        
        return sanitized;
    }
}