using TheBoys.Application.Common.Requests;
using TheBoys.Application.Common.Responses;

namespace TheBoys.Application.Abstractions.Services;

public interface IMailService
{
    Task<Response> SendEmailAsync(
        SendEmailRequest request,
        CancellationToken cancellationToken = default
    );
}
