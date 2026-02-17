using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using TheBoys.API.Contracts;
using TheBoys.API.ExternalServices.Email;
using TheBoys.API.Settings;

namespace ChatSystem.API.Middlewares;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    readonly IEmailService _emailService;
    readonly EmailSettings _emailSettings;

    public ExceptionHandlingMiddleware(
        IEmailService emailService,
        IOptions<EmailSettings> emailSettingsOptions
    )
    {
        _emailService = emailService;
        _emailSettings = emailSettingsOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var alarm = new StringBuilder();

            alarm.AppendLine($"REQUEST URL: {context.Request.GetDisplayUrl()}\n\n");
            alarm.AppendLine($"CONTENT-TYPE: {context.Request.ContentType}\n\n");
            alarm.AppendLine($"IP-ADDRESS: {context.Connection.RemoteIpAddress}\n\n");

            foreach (var headerKeyValuePair in context.Request.Headers)
                alarm.AppendLine(
                    $"{headerKeyValuePair.Key.ToUpper()}: {headerKeyValuePair.Value}\n\n"
                );

            alarm.AppendLine($"EXCEPTION-MESSAGE: {ex.Message}\n\n");
            alarm.AppendLine($"STACK-TRACE: {ex.StackTrace}\n\n");
            alarm.AppendLine($"SOURCE: {ex.Source}\n\n");
            alarm.AppendLine($"INNER-EXCEPTION-MESSAGE: {ex.InnerException?.Message}\n\n");
            alarm.AppendLine($"INNER-STACK-TRACE: {ex.InnerException?.StackTrace}\n\n");
            string requestBody = await ReadRequestBodyAsync(context);
            alarm.AppendLine($"REQUEST-BODY: {requestBody}\n");

            var emailContract = new MailAddressContract(
                _emailSettings.MailHost,
                _emailSettings.MailUsername,
                _emailSettings.MailPasswword,
                _emailSettings.MailPort,
                _emailSettings.MailSender,
                "The Boys Error",
                new List<string>() { _emailSettings.MailSender },
                "EXCEPTION HAPPPEN",
                alarm.ToString(),
                false
            );

            await _emailService.SendEmailAsync(emailContract);

            var statusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(
                new
                {
                    Success = false,
                    Message = "Oops!! something went wrong",
                    StatusCode = statusCode,
                }
            );
        }
    }

    private async Task<string> ReadRequestBodyAsync(HttpContext context)
    {
        try
        {
            if (context.Request.ContentLength == null || context.Request.ContentLength == 0)
                return "EMPTY BODY";

            context.Request.Body.Position = 0;

            using StreamReader reader =
                new(
                    context.Request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024,
                    leaveOpen: true
                );

            var bodyAsString = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            return System.Text.Json.JsonSerializer.Serialize(
                JsonDocument.Parse(bodyAsString),
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return "UNABLE TO READ BODY";
        }
    }
}
