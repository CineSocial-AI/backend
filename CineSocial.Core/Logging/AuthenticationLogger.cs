using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CineSocial.Core.Logging;

public class AuthenticationLogger : IAuthenticationLogger
{
    private readonly ILogger<AuthenticationLogger> _logger;

    public AuthenticationLogger(ILogger<AuthenticationLogger> logger)
    {
        _logger = logger;
    }

    public void LogSuccessfulLogin(string userId, string userName, string ipAddress, string? userAgent = null)
    {
        _logger.LogInformation(
            "Successful Login: User {UserId} ({UserName}) from {IpAddress} - UserAgent: {UserAgent}",
            userId, userName, ipAddress, userAgent ?? "Unknown");
    }

    public void LogFailedLogin(string email, string ipAddress, string reason, string? userAgent = null)
    {
        _logger.LogWarning(
            "Failed Login Attempt: Email {Email} from {IpAddress} - Reason: {Reason} - UserAgent: {UserAgent}",
            email, ipAddress, reason, userAgent ?? "Unknown");
    }

    public void LogLogout(string userId, string userName, string ipAddress)
    {
        _logger.LogInformation(
            "User Logout: User {UserId} ({UserName}) from {IpAddress}",
            userId, userName, ipAddress);
    }

    public void LogTokenRefresh(string userId, string userName, string ipAddress)
    {
        _logger.LogInformation(
            "Token Refresh: User {UserId} ({UserName}) from {IpAddress}",
            userId, userName, ipAddress);
    }

    public void LogTokenExpiration(string userId, string userName)
    {
        _logger.LogInformation(
            "Token Expired: User {UserId} ({UserName})",
            userId, userName);
    }

    public void LogPasswordChange(string userId, string userName, string ipAddress)
    {
        _logger.LogInformation(
            "Password Changed: User {UserId} ({UserName}) from {IpAddress}",
            userId, userName, ipAddress);
    }

    public void LogAccountLockout(string userId, string userName, string ipAddress, string reason)
    {
        _logger.LogWarning(
            "Account Lockout: User {UserId} ({UserName}) from {IpAddress} - Reason: {Reason}",
            userId, userName, ipAddress, reason);
    }

    public void LogUnauthorizedAccess(string? userId, string resource, string action, string ipAddress)
    {
        _logger.LogWarning(
            "Unauthorized Access Attempt: User {UserId} tried to {Action} on {Resource} from {IpAddress}",
            userId ?? "Anonymous", action, resource, ipAddress);
    }

    public void LogSuspiciousActivity(string description, string? userId, string ipAddress, Dictionary<string, object>? additionalData = null)
    {
        var additionalInfo = additionalData != null ? JsonSerializer.Serialize(additionalData) : "None";
        
        _logger.LogWarning(
            "Suspicious Activity: {Description} - User: {UserId} - IP: {IpAddress} - Additional Data: {AdditionalData}",
            description, userId ?? "Anonymous", ipAddress, additionalInfo);
    }
}