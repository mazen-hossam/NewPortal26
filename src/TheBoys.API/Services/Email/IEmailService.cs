using System.Threading;
using System.Threading.Tasks;
using TheBoys.API.Contracts;

namespace TheBoys.API.ExternalServices.Email;

public interface IEmailService
{
    Task<bool> SendEmailAsync(
        MailAddressContract mailContract,
        CancellationToken cancellationToken = default
    );
}
