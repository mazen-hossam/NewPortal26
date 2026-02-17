using TheBoys.Application.Enums;

namespace TheBoys.Application.Common.Requests;

public class SendEmailRequest
{
    public string Email { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public int RatingValue { get; set; }
    public MailType Type { get; set; }
}
