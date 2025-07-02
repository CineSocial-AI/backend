using CineSocial.Core.Logging;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Diagnostics;

namespace CineSocial.Infrastructure.Logging;

public class DatabaseCommandInterceptor : DbCommandInterceptor
{
    private readonly IDatabaseLogger _databaseLogger;
    private readonly ILogger<DatabaseCommandInterceptor> _logger;

    public DatabaseCommandInterceptor(IDatabaseLogger databaseLogger, ILogger<DatabaseCommandInterceptor> logger)
    {
        _databaseLogger = databaseLogger;
        _logger = logger;
    }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command, 
        CommandEventData eventData, 
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        LogCommandExecution(command, "NonQuery");
        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<int> NonQueryExecutedAsync(
        DbCommand command, 
        CommandExecutedEventData eventData, 
        int result,
        CancellationToken cancellationToken = default)
    {
        LogCommandCompleted(command, eventData.Duration, "NonQuery", result);
        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command, 
        CommandEventData eventData, 
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        LogCommandExecution(command, "Reader");
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command, 
        CommandExecutedEventData eventData, 
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        LogCommandCompleted(command, eventData.Duration, "Reader");
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command, 
        CommandEventData eventData, 
        InterceptionResult<object> result,
        CancellationToken cancellationToken = default)
    {
        LogCommandExecution(command, "Scalar");
        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<object> ScalarExecutedAsync(
        DbCommand command, 
        CommandExecutedEventData eventData, 
        object result,
        CancellationToken cancellationToken = default)
    {
        LogCommandCompleted(command, eventData.Duration, "Scalar");
        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
    {
        _databaseLogger.LogDatabaseError(
            GetOperationType(command), 
            eventData.Exception, 
            command.CommandText);
            
        _logger.LogError(eventData.Exception, 
            "Database command failed: {CommandType} - Duration: {Duration}ms",
            GetOperationType(command), eventData.Duration.TotalMilliseconds);
            
        base.CommandFailed(command, eventData);
    }

    public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        _databaseLogger.LogDatabaseError(
            GetOperationType(command), 
            eventData.Exception, 
            command.CommandText);
            
        _logger.LogError(eventData.Exception, 
            "Database command failed: {CommandType} - Duration: {Duration}ms",
            GetOperationType(command), eventData.Duration.TotalMilliseconds);
            
        return base.CommandFailedAsync(command, eventData, cancellationToken);
    }

    private void LogCommandExecution(DbCommand command, string operationType)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Executing {OperationType} command: {CommandText}", 
                operationType, SanitizeCommandText(command.CommandText));
        }
    }

    private void LogCommandCompleted(DbCommand command, TimeSpan duration, string operationType, object? result = null)
    {
        _databaseLogger.LogQuery(
            GetOperationType(command),
            command.CommandText,
            duration);

        if (result is int affectedRows && affectedRows > 0)
        {
            _logger.LogInformation(
                "Database {OperationType} completed: {AffectedRows} rows affected - Duration: {Duration}ms",
                operationType, affectedRows, duration.TotalMilliseconds);
        }
        else
        {
            _logger.LogDebug(
                "Database {OperationType} completed - Duration: {Duration}ms",
                operationType, duration.TotalMilliseconds);
        }
    }

    private string GetOperationType(DbCommand command)
    {
        var commandText = command.CommandText?.Trim().ToUpper() ?? "";
        
        return commandText switch
        {
            var text when text.StartsWith("SELECT") => "SELECT",
            var text when text.StartsWith("INSERT") => "INSERT",
            var text when text.StartsWith("UPDATE") => "UPDATE",
            var text when text.StartsWith("DELETE") => "DELETE",
            var text when text.StartsWith("CREATE") => "CREATE",
            var text when text.StartsWith("ALTER") => "ALTER",
            var text when text.StartsWith("DROP") => "DROP",
            _ => "UNKNOWN"
        };
    }

    private string SanitizeCommandText(string commandText)
    {
        if (string.IsNullOrEmpty(commandText)) return commandText;
        
        // Remove potential sensitive data from command text
        var sanitized = commandText;
        
        // Replace parameter values with placeholders for better readability
        var parameterPattern = @"@\w+";
        sanitized = System.Text.RegularExpressions.Regex.Replace(sanitized, parameterPattern, "?");
        
        return sanitized;
    }
}