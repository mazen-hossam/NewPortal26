using Microsoft.AspNetCore.Mvc;
using TheBoys.Application.Abstractions.Services;
using TheBoys.Application.Common.Requests;
using TheBoys.Application.Common.Responses;

namespace TheBoys.API.Controllers.Mails;

[Route("api/v1/mail")]
[ApiController]
public class MailController : ControllerBase
{
    private readonly IMailService _mailService;

    public MailController(IMailService mailService)
    {
        _mailService = mailService;
    }

    [HttpPost("send")]
    public async Task<ActionResult<Response>> SendEmailAsync(
        [FromBody] SendEmailRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _mailService.SendEmailAsync(request, cancellationToken);
        return Ok(result);
    }
}
