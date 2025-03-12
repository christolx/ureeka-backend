using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

public class SendGridEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SendGridEmailSender> _logger;

    public SendGridEmailSender(IConfiguration configuration, ILogger<SendGridEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(_configuration["SendGrid:FromEmail"], _configuration["SendGrid:FromName"]);
        var to = new EmailAddress(email);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlMessage);
        
        var response = await client.SendEmailAsync(msg);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send email through SendGrid. Status code: {StatusCode}", response.StatusCode);
        }
    }
}