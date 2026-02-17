using System.Net;
using System.Net.Mail;
using TheBoys.API.Contracts;

namespace TheBoys.API.ExternalServices.Email;

public class EmailService : IEmailService
{
    public async Task<bool> SendEmailAsync(
        MailAddressContract mailContract,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            using (var smtpClient = new SmtpClient())
            using (var mailMessage = new MailMessage())
            {
                #region Build Client
                smtpClient.Host = mailContract.Host;
                smtpClient.Port = mailContract.Port;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(
                    mailContract.UserName,
                    mailContract.Password
                );
                #endregion

                #region Build Message
                mailMessage.From = new MailAddress(mailContract.Sender, mailContract.DisplayName);
                mailMessage.Subject = mailContract.Subject;
                mailContract.ToEmails.ForEach(to => mailMessage.To.Add(new MailAddress(to)));
                mailMessage.Body = mailContract.Body;
                mailMessage.IsBodyHtml = mailContract.IsHtmlBody;
                #endregion

                await smtpClient.SendMailAsync(mailMessage, cancellationToken);
                return true;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
