using Microsoft.AspNetCore.Mvc;
using TheBoys.Application.Abstractions.Services;
using TheBoys.Application.Common.Responses;
using TheBoys.Application.Dtos;

namespace TheBoys.API.Controllers.News;

[Route("api/v1/news")]
[ApiController]
public class NewsStatisticsController : ControllerBase
{
    private readonly INewsStatisticsService _newsStatisticsService;

    public NewsStatisticsController(INewsStatisticsService newsStatisticsService)
    {
        _newsStatisticsService = newsStatisticsService;
    }

  
    //[HttpGet("statistics")]
    //public async Task<ActionResult<ResponseOf<NewsStatisticsResponseDto>>> GetAllAsync(
    //    [FromQuery] NewsStatisticsQueryDto request,
    //    CancellationToken cancellationToken = default
    //)
    //{
    //    var result = await _newsStatisticsService.GetAllStatisticsAsync(request, cancellationToken);
    //    return Ok(result);
    //}


    [HttpGet("statistics/overview")]
    public async Task<ActionResult<ResponseOf<NewsOverviewStatisticsResponseDto>>> GetOverviewAsync(
        [FromQuery] NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _newsStatisticsService.GetOverviewAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("statistics/languages")]
    public async Task<ActionResult<ResponseOf<NewsLanguageStatisticsResponseDto>>> GetLanguagesAsync(
        [FromQuery] NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _newsStatisticsService.GetLanguageStatisticsAsync(request, cancellationToken);
        return Ok(result);
    }

 
    [HttpGet("statistics/time")]
    public async Task<ActionResult<ResponseOf<NewsTimeStatisticsResponseDto>>> GetTimeAsync(
        [FromQuery] NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _newsStatisticsService.GetTimeStatisticsAsync(request, cancellationToken);
        return Ok(result);
    }

  
    [HttpGet("statistics/featured")]
    public async Task<ActionResult<ResponseOf<NewsFeaturedStatisticsResponseDto>>> GetFeaturedAsync(
        [FromQuery] NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _newsStatisticsService.GetFeaturedStatisticsAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("statistics/content-completeness")]
    public async Task<ActionResult<ResponseOf<NewsContentCompletenessStatisticsResponseDto>>> GetContentCompletenessAsync(
        [FromQuery] NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _newsStatisticsService.GetContentCompletenessStatisticsAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("statistics/sources")]
    public async Task<ActionResult<ResponseOf<NewsSourceStatisticsResponseDto>>> GetSourcesAsync(
        [FromQuery] NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _newsStatisticsService.GetSourceStatisticsAsync(request, cancellationToken);
        return Ok(result);
    }


    [HttpGet("statistics/recent")]
    public async Task<ActionResult<ResponseOf<NewsRecentStatisticsResponseDto>>> GetRecentAsync(
        [FromQuery] NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _newsStatisticsService.GetRecentStatisticsAsync(request, cancellationToken);
        return Ok(result);
    }


    [HttpGet("statistics/comparisons")]
    public async Task<ActionResult<ResponseOf<NewsComparisonStatisticsResponseDto>>> GetComparisonsAsync(
        [FromQuery] NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _newsStatisticsService.GetComparisonStatisticsAsync(request, cancellationToken);
        return Ok(result);
    }
}
