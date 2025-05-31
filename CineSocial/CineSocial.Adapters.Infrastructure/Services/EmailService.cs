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
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly string _password;
    private readonly bool _useTls;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _smtpHost = _configuration["EmailSettings:Host"] ?? throw new ArgumentNullException("SMTP Host not configured");
        _smtpPort = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
        _fromEmail = _configuration["EmailSettings:FromEmail"] ?? throw new ArgumentNullException("From Email not configured");
        _fromName = _configuration["EmailSettings:FromName"] ?? "CineSocial";
        _password = _configuration["EmailSettings:Password"] ?? throw new ArgumentNullException("Email Password not configured");
        _useTls = bool.Parse(_configuration["EmailSettings:UseTls"] ?? "true");
    }

    public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
    {
        var subject = "CineSocial - Email Doðrulama";
        var body = $@"
            <html>
            <body>
                <h2>CineSocial'e Hoþ Geldiniz!</h2>
                <p>Hesabýnýzý aktifleþtirmek için aþaðýdaki linke týklayýn:</p>
                <p><a href='{confirmationLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Email Adresimi Doðrula</a></p>
                <p>Eðer bu linke týklayamazsanýz, aþaðýdaki URL'yi tarayýcýnýza kopyalayýn:</p>
                <p>{confirmationLink}</p>
                <p>Bu link 24 saat geçerlidir.</p>
                <br>
                <p>CineSocial Ekibi</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetAsync(string email, string resetLink)
    {
        var subject = "CineSocial - Þifre Sýfýrlama";
        var body = $@"
            <html>
            <body>
                <h2>Þifre Sýfýrlama Ýsteði</h2>
                <p>CineSocial hesabýnýz için þifre sýfýrlama isteði aldýk.</p>
                <p>Yeni þifre oluþturmak için aþaðýdaki linke týklayýn:</p>
                <p><a href='{resetLink}' style='background-color: #dc3545; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Þifremi Sýfýrla</a></p>
                <p>Eðer bu linke týklayamazsanýz, aþaðýdaki URL'yi tarayýcýnýza kopyalayýn:</p>
                <p>{resetLink}</p>
                <p>Bu link 1 saat geçerlidir.</p>
                <p>Eðer bu isteði siz yapmadýysanýz, bu emaili görmezden gelebilirsiniz.</p>
                <br>
                <p>CineSocial Ekibi</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string email, string firstName)
    {
        var subject = "CineSocial'e Hoþ Geldiniz!";
        var body = $@"
            <html>
            <body>
                <h2>Merhaba {firstName}!</h2>
                <p>CineSocial'e katýldýðýnýz için teþekkür ederiz!</p>
                <p>Artýk:</p>
                <ul>
                    <li>Filmleri keþfedebilir ve inceleyebilirsiniz</li>
                    <li>Kendi film listelerinizi oluþturabilirsiniz</li>
                    <li>Diðer kullanýcýlarýn yorumlarýný okuyabilirsiniz</li>
                    <li>Size özel film önerileri alabilirsiniz</li>
                </ul>
                <p>Ýyi eðlenceler!</p>
                <br>
                <p>CineSocial Ekibi</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            using var client = new SmtpClient(_smtpHost, _smtpPort);
            client.EnableSsl = _useTls;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_fromEmail, _password);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);

            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            throw;
        }
    }
}