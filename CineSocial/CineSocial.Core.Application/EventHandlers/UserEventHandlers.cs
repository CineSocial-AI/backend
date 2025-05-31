using MediatR;
using Microsoft.Extensions.Logging;
using CineSocial.Core.Domain.Events;
using CineSocial.Core.Application.Contracts.Services;

namespace CineSocial.Core.Application.EventHandlers;

/// <summary>
/// User kayıt olduğunda hoş geldin emaili gönder
/// </summary>
public class SendWelcomeEmailHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SendWelcomeEmailHandler> _logger;

    public SendWelcomeEmailHandler(IEmailService emailService, ILogger<SendWelcomeEmailHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("📧 Sending welcome email to user {UserId} - {Email}",
                notification.UserId, notification.Email);

            await _emailService.SendWelcomeEmailAsync(notification.Email, notification.FirstName);

            _logger.LogInformation("✅ Welcome email sent successfully to {Email}", notification.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to send welcome email to {Email}: {Message}",
                notification.Email, ex.Message);

            // Don't throw - email failure shouldn't fail user registration
        }
    }
}

/// <summary>
/// User kayıt olduğunda log yaz
/// </summary>
public class LogUserRegistrationHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly ILogger<LogUserRegistrationHandler> _logger;

    public LogUserRegistrationHandler(ILogger<LogUserRegistrationHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🎉 New user registered successfully!");
        _logger.LogInformation("   User ID: {UserId}", notification.UserId);
        _logger.LogInformation("   Email: {Email}", notification.Email);
        _logger.LogInformation("   Name: {FirstName} {LastName}", notification.FirstName, notification.LastName);
        _logger.LogInformation("   Registration Time: {RegistrationTime}", notification.OccurredAt);

        // Simulate some async work (analytics, metrics, etc.)
        await Task.Delay(100, cancellationToken);

        _logger.LogInformation("✅ User registration logged successfully");
    }
}

/// <summary>
/// User profile güncellendiğinde log yaz
/// </summary>
public class LogUserProfileUpdateHandler : INotificationHandler<UserProfileUpdatedEvent>
{
    private readonly ILogger<LogUserProfileUpdateHandler> _logger;

    public LogUserProfileUpdateHandler(ILogger<LogUserProfileUpdateHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(UserProfileUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("👤 User profile updated:");
        _logger.LogInformation("   User ID: {UserId}", notification.UserId);
        _logger.LogInformation("   New Name: {FirstName} {LastName}", notification.FirstName, notification.LastName);
        _logger.LogInformation("   Update Time: {UpdateTime}", notification.OccurredAt);

        await Task.CompletedTask;
    }
}

/// <summary>
/// User deaktif edildiğinde log yaz ve cleanup yap
/// </summary>
public class HandleUserDeactivationHandler : INotificationHandler<UserDeactivatedEvent>
{
    private readonly ILogger<HandleUserDeactivationHandler> _logger;

    public HandleUserDeactivationHandler(ILogger<HandleUserDeactivationHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(UserDeactivatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🚫 User account deactivated:");
        _logger.LogInformation("   User ID: {UserId}", notification.UserId);
        _logger.LogInformation("   Email: {Email}", notification.Email);
        _logger.LogInformation("   Deactivation Time: {DeactivationTime}", notification.OccurredAt);

        // Burada cleanup işlemleri yapılabilir:
        // - Active sessions'ları sonlandır
        // - Cache'den kullanıcı bilgilerini temizle
        // - Analytics'e bildir
        // - Admin'lere bildirim gönder (eğer gerekiyorsa)

        _logger.LogInformation("✅ User deactivation handling completed");

        await Task.CompletedTask;
    }
}

/// <summary>
/// User giriş yaptığında log yaz (gelecekte eklenecek)
/// </summary>
public class LogUserLoginHandler : INotificationHandler<UserLoggedInEvent>
{
    private readonly ILogger<LogUserLoginHandler> _logger;

    public LogUserLoginHandler(ILogger<LogUserLoginHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(UserLoggedInEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔐 User logged in:");
        _logger.LogInformation("   User ID: {UserId}", notification.UserId);
        _logger.LogInformation("   Email: {Email}", notification.Email);
        _logger.LogInformation("   Login Time: {LoginTime}", notification.LoginTime);

        // Burada şunlar yapılabilir:
        // - Last login time güncelle
        // - Security analytics
        // - Suspicious login detection
        // - Login notifications (if enabled)

        await Task.CompletedTask;
    }
}