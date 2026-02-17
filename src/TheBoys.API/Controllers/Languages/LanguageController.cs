using Microsoft.AspNetCore.Mvc;
using TheBoys.Application.Abstractions.Services;
using TheBoys.Application.Common.Requests;
using TheBoys.Application.Common.Responses;
using TheBoys.Application.Dtos;

namespace TheBoys.API.Controllers.Languages;

[Route("api/v1/languages")]
[ApiController]
public class LanguageController : ControllerBase
{
    private readonly ILanguageService _languageService;

    public LanguageController(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResponse<List<LanguageDto>>>> GetAllAsync(
        [FromQuery] PaginateRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _languageService.GetAllAsync(request, cancellationToken);
        return Ok(result);
    }
}
