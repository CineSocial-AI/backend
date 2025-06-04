using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using CineSocial.Core.Application.Contracts.Services;

namespace CineSocial.Adapters.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly bool _enableSsl;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        _smtpHost = _configuration["EmailSettings:Host"] ?? "localhost";
        _smtpPort = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
        _smtpUsername = _configuration["EmailSettings:Username"] ?? "";
        _smtpPassword = _configuration["EmailSettings:Password"] ?? "";
        _enableSsl = bool.Parse(_configuration["EmailSettings:UseTls"] ?? "true");
        _fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@cinesocial.com";
        _fromName = _configuration["EmailSettings:FromName"] ?? "CineSocial";
    }

    public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
    {
        var subject = "CineSocial - Email Doğrulama";
        var body = $@"
            <html>
            <body>
                <h2>Hoş geldiniz!</h2>
                <p>CineSocial'e kaydolduğunuz için teşekkürler.</p>
                <p>Email adresinizi doğrulamak için aşağıdaki linke tıklayın:</p>
                <p><a href=""{confirmationLink}"" style=""background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;"">Email Doğrula</a></p>
                <p>Bu link 24 saat geçerlidir.</p>
                <br>
                <p>İyi seyirler!</p>
                <p>CineSocial Ekibi</p>
            </body>
            </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetAsync(string email, string resetLink)
    {
        var subject = "CineSocial - Şifre Sıfırlama";
        var body = $@"
            <html>
            <body>
                <h2>Şifre Sıfırlama</h2>
                <p>Şifrenizi sıfırlamak için bir talepte bulundunuz.</p>
                <p>Şifrenizi sıfırlamak için aşağıdaki linke tıklayın:</p>
                <p><a href=""{resetLink}"" style=""background-color: #dc3545; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;"">Şifreyi Sıfırla</a></p>
                <p>Bu link 1 saat geçerlidir.</p>
                <p>Eğer bu talebi siz yapmadıysanız, bu emaili dikkate almayın.</p>
                <br>
                <p>CineSocial Ekibi</p>
            </body>
            </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string email, string firstName)
    {
        var subject = "CineSocial'e Hoş Geldiniz!";
        var body = $@"
            <html>
            <body>
                <h2>Merhaba {firstName}!</h2>
                <p>CineSocial ailesine hoş geldiniz! 🎬</p>
                <p>Artık:</p>
                <ul>
                    <li>Filmleri keşfedebilir ve değerlendirebilirsiniz</li>
                    <li>Diğer kullanıcıların görüşlerini okuyabilirsiniz</li>
                    <li>Watchlist'inizde izlemek istediğiniz filmleri takip edebilirsiniz</li>
                    <li>Film toplulukları oluşturabilir veya katılabilirsiniz</li>
                </ul>
                <p>Hemen başlamak için platformumuzu ziyaret edin!</p>
                <br>
                <p>İyi seyirler!</p>
                <p>CineSocial Ekibi</p>
            </body>
            </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            _logger.LogInformation("Sending email to {Email} with subject: {Subject}", toEmail, subject);

            using var client = new SmtpClient(_smtpHost, _smtpPort);
            client.EnableSsl = _enableSsl;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

            using var message = new MailMessage();
            message.From = new MailAddress(_fromEmail, _fromName);
            message.To.Add(toEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            await client.SendMailAsync(message);

            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}: {Message}", toEmail, ex.Message);
            throw;
        }
    }
}