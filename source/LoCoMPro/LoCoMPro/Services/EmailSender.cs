using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace LoCoMPro.Services;

public class SendGridEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public SendGridEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        string sendGridApiKey = _configuration["EmailSender:ApiKey"];
        if (string.IsNullOrEmpty(sendGridApiKey))
        {
            throw new Exception("The 'SendGridApiKey' is not configured");
        }

        var client = new SendGridClient(sendGridApiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress(_configuration["EmailSender:SenderEmail"], _configuration["EmailSender:SenderName"]),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));

        var response = await client.SendEmailAsync(msg);
        if (response.IsSuccessStatusCode)
        {
            System.Diagnostics.Debug.WriteLine("Email queued successfully");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Failed to send email");
        }
    }
}
