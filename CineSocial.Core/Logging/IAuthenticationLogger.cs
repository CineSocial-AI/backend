namespace CineSocial.Core.Logging;

public interface IAuthenticationLogger
{
    void LogSuccessfulLogin(string userId, string userName, string ipAddress, string? userAgent = null);
    void LogFailedLogin(string email, string ipAddress, string reason, string? userAgent = null);
    void LogLogout(string userId, string userName, string ipAddress);
    void LogTokenRefresh(string userId, string userName, string ipAddress);
    void LogTokenExpiration(string userId, string userName);
    void LogPasswordChange(string userId, string userName, string ipAddress);
    void LogAccountLockout(string userId, string userName, string ipAddress, string reason);
    void LogUnauthorizedAccess(string? userId, string resource, string action, string ipAddress);
    void LogSuspiciousActivity(string description, string? userId, string ipAddress, Dictionary<string, object>? additionalData = null);
}