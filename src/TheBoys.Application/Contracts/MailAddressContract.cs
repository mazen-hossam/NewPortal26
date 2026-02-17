namespace TheBoys.Application.Contracts;

public sealed record MailAddressContract(
    string Host,
    string UserName,
    string Password,
    int Port,
    string Sender,
    string DisplayName,
    List<string> ToEmails,
    string Subject,
    string Body,
    bool IsHtmlBody
)
{
    public static MailAddressContract Create(
        string host,
        string userName,
        string password,
        int port,
        string sender,
        string displayName,
        List<string> toEmails,
        string subject,
        string body,
        bool isHtmlBody
    ) => new(host, userName, password, port, sender, displayName, toEmails, subject, body, isHtmlBody);
}
