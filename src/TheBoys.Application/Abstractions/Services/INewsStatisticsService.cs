using TheBoys.Application.Common.Responses;
using TheBoys.Application.Dtos;

namespace TheBoys.Application.Abstractions.Services;

public interface INewsStatisticsService
{
    Task<ResponseOf<NewsOverviewStatisticsResponseDto>> GetOverviewAsync(
        NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    );

    Task<ResponseOf<NewsLanguageStatisticsResponseDto>> GetLanguageStatisticsAsync(
        NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    );

    Task<ResponseOf<NewsTimeStatisticsResponseDto>> GetTimeStatisticsAsync(
        NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    );

    Task<ResponseOf<NewsFeaturedStatisticsResponseDto>> GetFeaturedStatisticsAsync(
        NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    );

    Task<ResponseOf<NewsContentCompletenessStatisticsResponseDto>> GetContentCompletenessStatisticsAsync(
        NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    );

    Task<ResponseOf<NewsSourceStatisticsResponseDto>> GetSourceStatisticsAsync(
        NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    );

    Task<ResponseOf<NewsRecentStatisticsResponseDto>> GetRecentStatisticsAsync(
        NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    );

    Task<ResponseOf<NewsComparisonStatisticsResponseDto>> GetComparisonStatisticsAsync(
        NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    );

    //Task<ResponseOf<NewsStatisticsResponseDto>> GetAllStatisticsAsync(
    //    NewsStatisticsQueryDto request,
    //    CancellationToken cancellationToken = default
    //);
}
