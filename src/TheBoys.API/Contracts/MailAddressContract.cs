using System.Collections.Generic;

namespace TheBoys.API.Contracts;

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
    ) =>
        new(
            Host,
            UserName,
            Password,
            Port,
            Sender,
            DisplayName,
            ToEmails,
            Subject,
            Body,
            IsHtmlBody
        );
}
