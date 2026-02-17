using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;
using TheBoys.Application.Abstractions.Services;
using TheBoys.Application.Common.Requests;
using TheBoys.Application.Common.Responses;
using TheBoys.Application.Enums;
using TheBoys.Application.Settings;

namespace TheBoys.Infrastructure.Services;

public class MailService : IMailService
{
    private readonly EmailSettings _emailSettings;

    public MailService(IOptions<EmailSettings> options)
    {
        _emailSettings = options.Value;
    }

    public async Task<Response> SendEmailAsync(
        SendEmailRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var response = new Response();
        if (!new EmailAddressAttribute().IsValid(request.Email))
        {
            response.SendBadRequest();
            return response;
        }

        var body = new StringBuilder(request.Body);
        if (request.Type == MailType.Rating)
        {
            body.AppendLine($"Rating: {request.RatingValue}");
        }

        var message = new MailMessage
        {
            From = new MailAddress(_emailSettings.MailSender, _emailSettings.SenderName),
            Subject = request.Subject,
            Body = body.ToString(),
            IsBodyHtml = false
        };
        message.To.Add(new MailAddress("ahmedadel1672003@gmail.com"));

        using var smtpClient = new SmtpClient
        {
            Host = _emailSettings.MailHost,
            Port = _emailSettings.MailPort,
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(
                _emailSettings.MailUsername,
                _emailSettings.MailPasswword
            )
        };

        await smtpClient.SendMailAsync(message, cancellationToken);

        response.SendSuccess();
        return response;
    }
}
