namespace CineSocial.Core.Logging;

public interface IDatabaseLogger
{
    void LogQuery(string operation, string query, TimeSpan duration, string? userId = null);
    void LogDatabaseError(string operation, Exception exception, string? query = null, string? userId = null);
    void LogEntityOperation(string entityName, string operation, object entityId, string? userId = null);
    void LogBulkOperation(string entityName, string operation, int affectedRows, TimeSpan duration, string? userId = null);
}