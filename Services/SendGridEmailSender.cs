using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

public class SendGridEmailSender : IEmailSender
{
    private readonly ILogger<SendGridEmailSender> _logger;

    public SendGridEmailSender(ILogger<SendGridEmailSender> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogError("SendGrid API key not found in environment variables");
            throw new InvalidOperationException("SendGrid API key not configured");
        }

        var client = new SendGridClient(apiKey);
        
        var fromEmail = Environment.GetEnvironmentVariable("SENDGRID_FROM_EMAIL");
        var fromName = Environment.GetEnvironmentVariable("SENDGRID_FROM_NAME");
        
        if (string.IsNullOrEmpty(fromEmail))
        {
            _logger.LogError("SendGrid from email not found in environment variables");
            throw new InvalidOperationException("SendGrid from email not configured");
        }
        
        var from = new EmailAddress(fromEmail, fromName);
        var to = new EmailAddress(email);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlMessage);
        
        var response = await client.SendEmailAsync(msg);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send email through SendGrid. Status code: {StatusCode}", response.StatusCode);
        }
    }
}