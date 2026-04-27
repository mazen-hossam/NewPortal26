using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TheBoys.Application.Abstractions.Services;
using TheBoys.Application.Common.Requests;
using TheBoys.Application.Common.Responses;
using TheBoys.Application.Dtos;

namespace TheBoys.API.Controllers.News;

[Route("api/v1/news")]
[ApiController]
public class NewsController : ControllerBase
{
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    [HttpGet("SectorsNews")]
    public async Task<ActionResult<PaginationResponse<List<NewsDto>>>> PaginateAllNewsAsync(
        [FromQuery] PaginateNewsRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _newsService.PaginateSectorNewsAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("NewsUniv")]
    public async Task<ActionResult<PaginationResponse<List<NewsDto>>>> PaginateAllUnivNewsAsync(
        [FromQuery] PaginateNewsRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _newsService.PaginateUniversityNewsAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("newSec/{id:int}/{lid:int}")]
    // [HttpGet("/api/v1/News/{id:int}/{lid:int}")]
    public async Task<ActionResult<ResponseOf<NewsDto>>> GetNewSecAsync(
        [Required][FromRoute] int id,
        [Required][FromRoute] int lid,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _newsService.GetSectorNewsByIdAsync(id, lid, cancellationToken);
        return Ok(result);
    }

    [HttpGet("newUniv/{id:int}/{lid:int}")]
    public async Task<ActionResult<ResponseOf<NewsDto>>> GetNewUnivAsync(
        [Required][FromRoute] int id,
        [Required][FromRoute] int lid,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _newsService.GetUniversityNewsByIdAsync(id, lid, cancellationToken);
        return Ok(result);
    }

    [HttpGet("SearchAbbreviation")]
    public async Task<ActionResult<PaginationResponse<List<NewsDto>>>> SearchByOwnerAbbreviation(
        [FromQuery] SearchByOwnerAbbreviationRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _newsService.SearchByOwnerAbbreviationAsync(request, cancellationToken);
        return Ok(result);
    }
}
