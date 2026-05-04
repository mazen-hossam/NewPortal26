using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using TheBoys.Application.Abstractions.Services;
using TheBoys.Application.Common.Requests;
using TheBoys.Application.Common.Responses;
using TheBoys.Application.Dtos;

namespace TheBoys.API.Controllers.FacultyNews;

[Route("api/faculty-news")]
[ApiController]
public sealed class FacultyNewsController : ControllerBase
{
    private const int DefaultLanguageId = 1;
    private readonly IFacultyNewsService _facultyNewsService;

    public FacultyNewsController(IFacultyNewsService facultyNewsService)
    {
        _facultyNewsService = facultyNewsService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResponse<List<FacultyNewsItemDto>>>> GetPublishedAsync(
        [FromQuery] string fac,
        [FromQuery] string langId,
        [FromQuery] PaginateRequest request,
        CancellationToken cancellationToken = default
    )
    {
        request ??= new PaginateRequest();

        if (!TryNormalizeRequest(fac, langId, out var publicFacultyCode, out var languageId))
        {
            return Ok(InvalidPaginationResponse(request));
        }

        var result = await _facultyNewsService.GetPublishedAsync(
            publicFacultyCode,
            languageId,
            request,
            cancellationToken
        );

        return Ok(result);
    }

    [HttpGet("details")]
    public async Task<ActionResult<ResponseOf<FacultyNewsDetailsDto>>> GetDetailsAsync(
        [FromQuery] string fac,
        [FromQuery] string id,
        [FromQuery] string langId,
        CancellationToken cancellationToken = default
    )
    {
        if (
            !TryNormalizeRequest(fac, langId, out var publicFacultyCode, out var languageId)
            || !TryParsePositiveInt(id, out var newsId)
        )
        {
            return Ok(InvalidDetailsResponse());
        }

        var result = await _facultyNewsService.GetDetailsAsync(
            publicFacultyCode,
            newsId,
            languageId,
            cancellationToken
        );

        return Ok(result);
    }

    private static bool TryNormalizeRequest(
        string fac,
        string langId,
        out int publicFacultyCode,
        out int languageId
    )
    {
        languageId = DefaultLanguageId;

        if (!TryParsePositiveInt(fac, out publicFacultyCode))
        {
            return false;
        }

        return string.IsNullOrWhiteSpace(langId)
            || TryParsePositiveInt(langId, out languageId);
    }

    private static bool TryParsePositiveInt(string value, out int result)
    {
        result = 0;
        return !string.IsNullOrWhiteSpace(value)
            && int.TryParse(
                value.Trim(),
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out result
            )
            && result > 0;
    }

    private static PaginationResponse<List<FacultyNewsItemDto>> InvalidPaginationResponse(
        PaginateRequest request
    )
    {
        var response = new PaginationResponse<List<FacultyNewsItemDto>>
        {
            Result = new List<FacultyNewsItemDto>(),
            PageIndex = request?.PageIndex > 0 ? request.PageIndex : 1,
            PageSize = NormalizePageSize(request?.PageSize ?? 10)
        };

        response.SendBadRequest("Invalid request.");
        return response;
    }

    private static int NormalizePageSize(int pageSize)
    {
        if (pageSize < 1)
        {
            return 1;
        }

        return pageSize > 10 ? 10 : pageSize;
    }

    private static ResponseOf<FacultyNewsDetailsDto> InvalidDetailsResponse()
    {
        var response = new ResponseOf<FacultyNewsDetailsDto>();
        response.SendBadRequest("Invalid request.");
        return response;
    }
}
