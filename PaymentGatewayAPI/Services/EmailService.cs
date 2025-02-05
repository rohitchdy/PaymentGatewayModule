using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PaymentGatewayAPI.Configurations;
using PaymentGatewayAPI.Interfaces;

namespace PaymentGatewayAPI.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailConfig;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailConfig, ILogger<EmailService> logger)
    {
        _emailConfig = emailConfig.Value;
        _logger = logger;
    }


    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailConfig.SenderName, _emailConfig.SenderEmail));
            email.To.Add(new MailboxAddress("", to));
            email.Subject = subject;

            email.Body = new TextPart("html")
            {
                Text = body
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailConfig.SenderEmail, _emailConfig.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Email sending failed.", ex.ToString());
            return false;
        }
    }
}
