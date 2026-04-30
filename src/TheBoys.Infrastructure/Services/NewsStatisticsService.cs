using Microsoft.EntityFrameworkCore;
using TheBoys.Application.Abstractions.Services;
using TheBoys.Application.Common.Responses;
using TheBoys.Application.Dtos;
using TheBoys.Application.Extensions;
using TheBoys.Application.Misc;
using TheBoys.Domain.Entities;
using TheBoys.Infrastructure.Persistence;

namespace TheBoys.Infrastructure.Services;

public class NewsStatisticsService : INewsStatisticsService
{
    private const string UniversityNewsListingImageBasePath =
        "https://mu.menofia.edu.eg/PrtlFiles/uni/Portal/Images/";

    private readonly ApplicationDbContext _context;

    public NewsStatisticsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseOf<NewsOverviewStatisticsResponseDto>> GetOverviewAsync(
        NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    )
    {
        return await CreateSectionResponseAsync(
            request,
            async scope => new NewsOverviewStatisticsResponseDto
            {
                Filters = BuildAppliedFilters(request),
                Overview = await BuildOverviewAsync(scope, cancellationToken)
            }
        );
    }

    public async Task<ResponseOf<NewsLanguageStatisticsResponseDto>> GetLanguageStatisticsAsync(
        NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    )
    {
        return await CreateSectionResponseAsync(
            request,
            async scope => new NewsLanguageStatisticsResponseDto
            {
                Filters = BuildAppliedFilters(request),
                Languages = await BuildLanguageStatisticsAsync(scope, cancellationToken)
            }
        );
    }

    public async Task<ResponseOf<NewsTimeStatisticsResponseDto>> GetTimeStatisticsAsync(
        NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    )
    {
        return await CreateSectionResponseAsync(
            request,
            async scope => new NewsTimeStatisticsResponseDto
            {
                Filters = BuildAppliedFilters(request),
                Time = await BuildTimeStatisticsAsync(scope, cancellationToken)
            }
        );
    }

    public async Task<ResponseOf<NewsFeaturedStatisticsResponseDto>> GetFeaturedStatisticsAsync(
        NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    )
    {
        return await CreateSectionResponseAsync(
            request,
            async scope => new NewsFeaturedStatisticsResponseDto
            {
                Filters = BuildAppliedFilters(request),
                Featured = await BuildFeaturedStatisticsAsync(scope, cancellationToken)
            }
        );
    }

    public async Task<ResponseOf<NewsContentCompletenessStatisticsResponseDto>> GetContentCompletenessStatisticsAsync(
        NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    )
    {
        return await CreateSectionResponseAsync(
            request,
            async scope => new NewsContentCompletenessStatisticsResponseDto
            {
                Filters = BuildAppliedFilters(request),
                ContentCompleteness = await BuildContentCompletenessStatisticsAsync(scope, cancellationToken)
            }
        );
    }

    public async Task<ResponseOf<NewsSourceStatisticsResponseDto>> GetSourceStatisticsAsync(
        NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    )
    {
        return await CreateSectionResponseAsync(
            request,
            async scope => new NewsSourceStatisticsResponseDto
            {
                Filters = BuildAppliedFilters(request),
                Sources = await BuildSourceStatisticsAsync(scope, cancellationToken)
            }
        );
    }

    public async Task<ResponseOf<NewsRecentStatisticsResponseDto>> GetRecentStatisticsAsync(
        NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    )
    {
        return await CreateSectionResponseAsync(
            request,
            async scope => new NewsRecentStatisticsResponseDto
            {
                Filters = BuildAppliedFilters(request),
                Recent = await BuildRecentStatisticsAsync(scope, cancellationToken)
            }
        );
    }

    public async Task<ResponseOf<NewsComparisonStatisticsResponseDto>> GetComparisonStatisticsAsync(
        NewsStatisticsQueryDto request,
        CancellationToken cancellationToken = default
    )
    {
        return await CreateSectionResponseAsync(
            request,
            async scope => new NewsComparisonStatisticsResponseDto
            {
                Filters = BuildAppliedFilters(request),
                Comparisons = await BuildComparisonStatisticsAsync(scope, cancellationToken)
            }
        );
    }

    //public async Task<ResponseOf<NewsStatisticsResponseDto>> GetAllStatisticsAsync(
    //    NewsStatisticsQueryDto request,
    //    CancellationToken cancellationToken = default
    //)
    //{
    //    return await CreateSectionResponseAsync(
    //        request,
    //        async scope => new NewsStatisticsResponseDto
    //        {
    //            Filters = BuildAppliedFilters(request),
    //            Overview = await BuildOverviewAsync(scope, cancellationToken),
    //            Languages = await BuildLanguageStatisticsAsync(scope, cancellationToken),
    //            Time = await BuildTimeStatisticsAsync(scope, cancellationToken),
    //            Featured = await BuildFeaturedStatisticsAsync(scope, cancellationToken),
    //            ContentCompleteness = await BuildContentCompletenessStatisticsAsync(scope, cancellationToken),
    //            Sources = await BuildSourceStatisticsAsync(scope, cancellationToken),
    //            Recent = await BuildRecentStatisticsAsync(scope, cancellationToken),
    //            Comparisons = await BuildComparisonStatisticsAsync(scope, cancellationToken)
    //        }
    //    );
    //}

    private async Task<ResponseOf<TResponse>> CreateSectionResponseAsync<TResponse>(
        NewsStatisticsQueryDto request,
        Func<NewsStatisticsScope, Task<TResponse>> factory
    )
    {
        var response = new ResponseOf<TResponse>();
        var validationMessage = ValidateRequest(request);
        if (validationMessage.HasValue())
        {
            response.SendBadRequest(validationMessage);
            return response;
        }

        var scope = BuildScope(request);
        response.Result = await factory(scope);
        response.Success = true;
        return response;
    }

    private NewsStatisticsScope BuildScope(NewsStatisticsQueryDto request)
    {
        var hasLanguageFilter = request.LanguageId.HasValue;
        var languageId = request.LanguageId.GetValueOrDefault();
        var hasSearchFilter = request.Search.HasValue();
        var searchPattern = hasSearchFilter ? $"%{request.Search.Trim()}%" : string.Empty;
        var hasSourceFilter = request.Source.HasValue();
        var sourcePattern = hasSourceFilter ? $"%{request.Source.Trim()}%" : string.Empty;

        var filteredNewsQuery = ApplyDateFilters(
            ApplyCommonNewsFilters(_context.NewsUnivs.AsNoTracking(), request),
            request
        );
        var newsQueryWithoutDateFilters = ApplyCommonNewsFilters(
            _context.NewsUnivs.AsNoTracking(),
            request
        );
        var filteredTranslationsQuery = ApplyCommonTranslationFilters(
            _context.NewsUnivsTranslations.AsNoTracking(),
            request
        ).Where(t => filteredNewsQuery.Select(n => n.NewsId).Contains(t.NewsId));

        return new NewsStatisticsScope
        {
            Request = request,
            Today = DateTime.Today,
            HasLanguageFilter = hasLanguageFilter,
            LanguageId = languageId,
            HasSearchFilter = hasSearchFilter,
            SearchPattern = searchPattern,
            HasSourceFilter = hasSourceFilter,
            SourcePattern = sourcePattern,
            FilteredNewsQuery = filteredNewsQuery,
            NewsQueryWithoutDateFilters = newsQueryWithoutDateFilters,
            FilteredTranslationsQuery = filteredTranslationsQuery
        };
    }

    private async Task<NewsOverviewStatisticsDto> BuildOverviewAsync(
        NewsStatisticsScope scope,
        CancellationToken cancellationToken
    )
    {
        var totalNewsCount = await scope.FilteredNewsQuery.CountAsync(cancellationToken);
        var totalTranslationsCount = await scope.FilteredTranslationsQuery.CountAsync(cancellationToken);
        var totalLanguagesCount = await _context.Languages.AsNoTracking().CountAsync(cancellationToken);
        var featuredNewsCount = await scope.FilteredNewsQuery.CountAsync(n => n.IsFeatured, cancellationToken);
        var newsWithImageCount = await scope.FilteredNewsQuery.CountAsync(
            n => n.NewsImg != null && n.NewsImg.Trim() != string.Empty,
            cancellationToken
        );
        var perNewsMetrics = await BuildPerNewsMetricsQuery(scope)
            .ToListAsync(cancellationToken);
        var languageCounts = perNewsMetrics.Select(x => x.LanguagesCount).ToList();

        return new NewsOverviewStatisticsDto
        {
            TotalNewsCount = totalNewsCount,
            TotalTranslationsCount = totalTranslationsCount,
            TotalLanguagesCount = totalLanguagesCount,
            FeaturedNewsCount = featuredNewsCount,
            NotFeaturedNewsCount = totalNewsCount - featuredNewsCount,
            NewsWithImageCount = newsWithImageCount,
            NewsWithoutImageCount = totalNewsCount - newsWithImageCount,
            AverageTranslationsPerNews = RoundValue(totalNewsCount == 0
                ? 0
                : (double)totalTranslationsCount / totalNewsCount),
            NewsWithSingleLanguageCount = languageCounts.Count(x => x == 1),
            NewsWithMultipleLanguagesCount = languageCounts.Count(x => x > 1),
            MaxLanguagesForSingleNews = languageCounts.Count == 0 ? 0 : languageCounts.Max(),
            MinLanguagesForSingleNews = languageCounts.Count == 0 ? 0 : languageCounts.Min()
        };
    }

    private async Task<NewsLanguageStatisticsDto> BuildLanguageStatisticsAsync(
        NewsStatisticsScope scope,
        CancellationToken cancellationToken
    )
    {
        var totalNewsCount = await scope.FilteredNewsQuery.CountAsync(cancellationToken);
        var perNewsMetrics = await BuildPerNewsMetricsQuery(scope).ToListAsync(cancellationToken);
        var newsCountByLanguageData = await scope.FilteredTranslationsQuery
            .GroupBy(t => t.LangId)
            .Select(g => new
            {
                LanguageId = g.Key,
                TranslationsCount = g.Count(),
                NewsCount = g.Select(x => x.NewsId).Distinct().Count()
            })
            .ToListAsync(cancellationToken);
        var languageTable = await _context.Languages.AsNoTracking().ToListAsync(cancellationToken);
        var languageLookup = BuildLanguageLookup(languageTable);
        var languageItems = newsCountByLanguageData
            .Select(x =>
            {
                var languageInfo = ResolveLanguage(x.LanguageId, languageLookup);
                return new NewsStatisticsLanguageItemDto
                {
                    LanguageId = x.LanguageId,
                    LanguageCode = languageInfo.Code,
                    LanguageName = languageInfo.Name,
                    NewsCount = x.NewsCount,
                    TranslationsCount = x.TranslationsCount,
                    Percentage = RoundPercentage(totalNewsCount == 0
                        ? 0
                        : (double)x.NewsCount * 100 / totalNewsCount)
                };
            })
            .ToList();
        var languageItemsByNews = languageItems
            .OrderByDescending(x => x.NewsCount)
            .ThenBy(x => x.LanguageName)
            .ToList();
        var languageItemsByTranslations = languageItems
            .OrderByDescending(x => x.TranslationsCount)
            .ThenBy(x => x.LanguageName)
            .ToList();
        var missingLanguageIds = languageTable
            .Where(x => newsCountByLanguageData.All(s => s.LanguageId != x.Id))
            .Select(ToLanguageDto)
            .ToList();

        return new NewsLanguageStatisticsDto
        {
            NewsCountByLanguage = languageItemsByNews,
            TranslationsCountByLanguage = languageItemsByTranslations,
            PercentageByLanguage = languageItemsByNews
                .Select(x => new NewsStatisticsLanguagePercentageDto
                {
                    LanguageId = x.LanguageId,
                    LanguageCode = x.LanguageCode,
                    LanguageName = x.LanguageName,
                    Percentage = x.Percentage
                })
                .ToList(),
            MostUsedLanguage = languageItemsByNews.FirstOrDefault(),
            LeastUsedLanguage = languageItemsByNews
                .OrderBy(x => x.NewsCount)
                .ThenBy(x => x.TranslationsCount)
                .ThenBy(x => x.LanguageName)
                .FirstOrDefault(),
            LanguagesWithNoNews = missingLanguageIds,
            LanguageDistributionPerNews = perNewsMetrics
                .GroupBy(x => x.LanguagesCount)
                .Select(g => new NewsStatisticsDistributionItemDto
                {
                    LanguagesCount = g.Key,
                    NewsCount = g.Count()
                })
                .OrderBy(x => x.LanguagesCount)
                .ToList()
        };
    }

    private async Task<NewsTimeStatisticsDto> BuildTimeStatisticsAsync(
        NewsStatisticsScope scope,
        CancellationToken cancellationToken
    )
    {
        var datedNewsQuery = scope.FilteredNewsQuery.Where(n => n.NewsDate.HasValue);
        var newsCountByYear = new List<NewsStatisticsYearCountDto>();
        var newsCountByMonth = new List<NewsStatisticsMonthCountDto>();
        var newsCountByDay = new List<NewsStatisticsDayCountDto>();

        if (!scope.Request.Year.HasValue && !scope.Request.FromDate.HasValue && !scope.Request.ToDate.HasValue)
        {
            newsCountByYear = await datedNewsQuery
                .GroupBy(n => n.NewsDate!.Value.Year)
                .Select(g => new NewsStatisticsYearCountDto
                {
                    Year = g.Key,
                    NewsCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ToListAsync(cancellationToken);
        }

        if (!scope.Request.Year.HasValue || scope.Request.Month.HasValue || scope.Request.FromDate.HasValue || scope.Request.ToDate.HasValue)
        {
            newsCountByDay = await BuildDaySeriesQuery(datedNewsQuery, scope.Request)
                .ToListAsync(cancellationToken);
        }

        if (!scope.Request.Year.HasValue || !scope.Request.Month.HasValue)
        {
            newsCountByMonth = await datedNewsQuery
                .GroupBy(n => new { n.NewsDate!.Value.Year, n.NewsDate.Value.Month })
                .Select(g => new NewsStatisticsMonthCountDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Label = $"{g.Key.Year:D4}-{g.Key.Month:D2}",
                    NewsCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync(cancellationToken);
        }
        else if (scope.Request.Year.HasValue && !scope.Request.FromDate.HasValue && !scope.Request.ToDate.HasValue)
        {
            newsCountByMonth = await datedNewsQuery
                .GroupBy(n => new { n.NewsDate!.Value.Year, n.NewsDate.Value.Month })
                .Select(g => new NewsStatisticsMonthCountDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Label = $"{g.Key.Year:D4}-{g.Key.Month:D2}",
                    NewsCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync(cancellationToken);
        }

        var currentMonthPeriod = GetMonthPeriod(scope.Today.Year, scope.Today.Month);
        var previousMonthAnchor = currentMonthPeriod.From.AddMonths(-1);
        var previousMonthPeriod = GetMonthPeriod(previousMonthAnchor.Year, previousMonthAnchor.Month);
        var currentYearPeriod = (new DateTime(scope.Today.Year, 1, 1), new DateTime(scope.Today.Year, 12, 31));
        var previousYearPeriod = (new DateTime(scope.Today.Year - 1, 1, 1), new DateTime(scope.Today.Year - 1, 12, 31));
        var currentMonthCount = await ApplyDateRange(
                scope.NewsQueryWithoutDateFilters,
                currentMonthPeriod.From,
                currentMonthPeriod.To
            )
            .CountAsync(cancellationToken);
        var previousMonthCount = await ApplyDateRange(
                scope.NewsQueryWithoutDateFilters,
                previousMonthPeriod.From,
                previousMonthPeriod.To
            )
            .CountAsync(cancellationToken);
        var currentYearCount = await ApplyDateRange(
                scope.NewsQueryWithoutDateFilters,
                currentYearPeriod.Item1,
                currentYearPeriod.Item2
            )
            .CountAsync(cancellationToken);
        var previousYearCount = await ApplyDateRange(
                scope.NewsQueryWithoutDateFilters,
                previousYearPeriod.Item1,
                previousYearPeriod.Item2
            )
            .CountAsync(cancellationToken);

        return new NewsTimeStatisticsDto
        {
            NewsCountByYear = newsCountByYear,
            NewsCountByMonth = newsCountByMonth,
            NewsCountByDay = newsCountByDay,
            NewsCountLast7Days = await ApplyDateRange(scope.FilteredNewsQuery, scope.Today.AddDays(-6), scope.Today)
                .CountAsync(cancellationToken),
            NewsCountLast30Days = await ApplyDateRange(scope.FilteredNewsQuery, scope.Today.AddDays(-29), scope.Today)
                .CountAsync(cancellationToken),
            NewsCountThisMonth = await ApplyDateRange(
                    scope.FilteredNewsQuery,
                    currentMonthPeriod.From,
                    currentMonthPeriod.To
                )
                .CountAsync(cancellationToken),
            NewsCountThisYear = await ApplyDateRange(
                    scope.FilteredNewsQuery,
                    currentYearPeriod.Item1,
                    currentYearPeriod.Item2
                )
                .CountAsync(cancellationToken),
            BusiestDay = newsCountByDay
                .OrderByDescending(x => x.NewsCount)
                .ThenByDescending(x => x.Date)
                .FirstOrDefault(),
            BusiestMonth = newsCountByMonth
                .OrderByDescending(x => x.NewsCount)
                .ThenByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .FirstOrDefault(),
            MonthlyGrowthPercentage = CalculateGrowthPercentage(currentMonthCount, previousMonthCount),
            YearlyGrowthPercentage = CalculateGrowthPercentage(currentYearCount, previousYearCount)
        };
    }

    private async Task<NewsFeaturedStatisticsDto> BuildFeaturedStatisticsAsync(
        NewsStatisticsScope scope,
        CancellationToken cancellationToken
    )
    {
        var totalNewsCount = await scope.FilteredNewsQuery.CountAsync(cancellationToken);
        var featuredCount = await scope.FilteredNewsQuery.CountAsync(n => n.IsFeatured, cancellationToken);
        var latestFeaturedNews = FinalizeRecentNewsItems(await BuildLatestNewsQuery(
                scope.FilteredNewsQuery.Where(n => n.IsFeatured),
                scope
            )
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .Take(scope.Request.RecentCount)
            .ToListAsync(cancellationToken));

        return new NewsFeaturedStatisticsDto
        {
            FeaturedCount = featuredCount,
            NonFeaturedCount = totalNewsCount - featuredCount,
            FeaturedPercentage = RoundPercentage(totalNewsCount == 0
                ? 0
                : (double)featuredCount * 100 / totalNewsCount),
            FeaturedByMonth = await scope.FilteredNewsQuery
                .Where(n => n.IsFeatured && n.NewsDate.HasValue)
                .GroupBy(n => new { n.NewsDate!.Value.Year, n.NewsDate.Value.Month })
                .Select(g => new NewsStatisticsMonthCountDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Label = $"{g.Key.Year:D4}-{g.Key.Month:D2}",
                    NewsCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync(cancellationToken),
            LatestFeaturedNews = latestFeaturedNews
        };
    }

    private async Task<NewsContentCompletenessStatisticsDto> BuildContentCompletenessStatisticsAsync(
        NewsStatisticsScope scope,
        CancellationToken cancellationToken
    )
    {
        var totalNewsCount = await scope.FilteredNewsQuery.CountAsync(cancellationToken);
        var totalTranslationsCount = await scope.FilteredTranslationsQuery.CountAsync(cancellationToken);
        var newsWithImageCount = await scope.FilteredNewsQuery.CountAsync(
            n => n.NewsImg != null && n.NewsImg.Trim() != string.Empty,
            cancellationToken
        );
        var translationsWithHeadCount = await scope.FilteredTranslationsQuery.CountAsync(
            t => t.NewsHead != null && t.NewsHead.Trim() != string.Empty,
            cancellationToken
        );
        var translationsWithAbbrCount = await scope.FilteredTranslationsQuery.CountAsync(
            t => t.NewsAbbr != null && t.NewsAbbr.Trim() != string.Empty,
            cancellationToken
        );
        var translationsWithBodyCount = await scope.FilteredTranslationsQuery.CountAsync(
            t => t.NewsBody != null && t.NewsBody.Trim() != string.Empty,
            cancellationToken
        );
        var translationsWithSourceCount = await scope.FilteredTranslationsQuery.CountAsync(
            t => t.NewsSource != null && t.NewsSource.Trim() != string.Empty,
            cancellationToken
        );
        var translationsWithImgAltCount = await scope.FilteredTranslationsQuery.CountAsync(
            t => t.ImgAlt != null && t.ImgAlt.Trim() != string.Empty,
            cancellationToken
        );
        var expectedFieldCount = totalNewsCount + (totalTranslationsCount * 5);
        var completedFieldCount = newsWithImageCount
            + translationsWithHeadCount
            + translationsWithAbbrCount
            + translationsWithBodyCount
            + translationsWithSourceCount
            + translationsWithImgAltCount;

        return new NewsContentCompletenessStatisticsDto
        {
            NewsWithImageCount = newsWithImageCount,
            NewsWithoutImageCount = totalNewsCount - newsWithImageCount,
            TranslationsWithHeadCount = translationsWithHeadCount,
            TranslationsMissingHeadCount = totalTranslationsCount - translationsWithHeadCount,
            TranslationsWithAbbrCount = translationsWithAbbrCount,
            TranslationsMissingAbbrCount = totalTranslationsCount - translationsWithAbbrCount,
            TranslationsWithBodyCount = translationsWithBodyCount,
            TranslationsMissingBodyCount = totalTranslationsCount - translationsWithBodyCount,
            TranslationsWithSourceCount = translationsWithSourceCount,
            TranslationsMissingSourceCount = totalTranslationsCount - translationsWithSourceCount,
            TranslationsWithImgAltCount = translationsWithImgAltCount,
            TranslationsMissingImgAltCount = totalTranslationsCount - translationsWithImgAltCount,
            CompletionRatePercentage = RoundPercentage(expectedFieldCount == 0
                ? 0
                : (double)completedFieldCount * 100 / expectedFieldCount)
        };
    }

    private async Task<NewsSourceStatisticsDto> BuildSourceStatisticsAsync(
        NewsStatisticsScope scope,
        CancellationToken cancellationToken
    )
    {
        var totalTranslationsCount = await scope.FilteredTranslationsQuery.CountAsync(cancellationToken);
        var translationsWithSourceCount = await scope.FilteredTranslationsQuery.CountAsync(
            t => t.NewsSource != null && t.NewsSource.Trim() != string.Empty,
            cancellationToken
        );
        var sourceStats = await scope.FilteredTranslationsQuery
            .GroupBy(t => t.NewsSource)
            .Select(g => new
            {
                Source = g.Key,
                TranslationsCount = g.Count(),
                NewsCount = g.Select(x => x.NewsId).Distinct().Count()
            })
            .ToListAsync(cancellationToken);
        var sourceItemsByNews = sourceStats
            .Where(x => !string.IsNullOrWhiteSpace(x.Source))
            .Select(x => new NewsStatisticsSourceItemDto
            {
                Source = StringExtensions.StripHtml(x.Source),
                NewsCount = x.NewsCount,
                TranslationsCount = x.TranslationsCount
            })
            .OrderByDescending(x => x.NewsCount)
            .ThenBy(x => x.Source)
            .ToList();
        var sourceItemsByTranslations = sourceItemsByNews
            .OrderByDescending(x => x.TranslationsCount)
            .ThenBy(x => x.Source)
            .ToList();

        return new NewsSourceStatisticsDto
        {
            NewsCountBySource = sourceItemsByNews,
            TranslationsCountBySource = sourceItemsByTranslations,
            MostCommonSource = sourceItemsByTranslations.FirstOrDefault(),
            MissingSourceCount = totalTranslationsCount - translationsWithSourceCount
        };
    }

    private async Task<NewsRecentStatisticsDto> BuildRecentStatisticsAsync(
        NewsStatisticsScope scope,
        CancellationToken cancellationToken
    )
    {
        var languageTable = await _context.Languages.AsNoTracking().ToListAsync(cancellationToken);
        var languageLookup = BuildLanguageLookup(languageTable);
        var latestNews = FinalizeRecentNewsItems(await BuildLatestNewsQuery(scope.FilteredNewsQuery, scope)
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .Take(scope.Request.RecentCount)
            .ToListAsync(cancellationToken));
        var latestFeaturedNews = FinalizeRecentNewsItems(await BuildLatestNewsQuery(
                scope.FilteredNewsQuery.Where(n => n.IsFeatured),
                scope
            )
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .Take(scope.Request.RecentCount)
            .ToListAsync(cancellationToken));
        var topNewsByLanguagesCount = FinalizeRecentNewsItems(await BuildLatestNewsQuery(
                scope.FilteredNewsQuery,
                scope
            )
            .OrderByDescending(x => x.LanguagesCount)
            .ThenByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .Take(scope.Request.RecentCount)
            .ToListAsync(cancellationToken));
        var latestTranslations = FinalizeRecentTranslationItems(
            await scope.FilteredTranslationsQuery
                .OrderByDescending(t => t.NewsUniv.NewsDate)
                .ThenByDescending(t => t.Id)
                .Select(t => new RecentTranslationProjection
                {
                    TranslationId = t.Id,
                    NewsId = t.NewsId,
                    Date = t.NewsUniv.NewsDate,
                    IsFeatured = t.NewsUniv.IsFeatured,
                    LanguageId = t.LangId,
                    LanguageCode = t.Language != null ? t.Language.LCID : string.Empty,
                    LanguageName = t.Language != null ? t.Language.LCID : string.Empty,
                    Title = t.NewsHead,
                    Source = t.NewsSource
                })
                .Take(scope.Request.RecentCount)
                .ToListAsync(cancellationToken),
            languageLookup
        );

        return new NewsRecentStatisticsDto
        {
            LatestNews = latestNews,
            LatestFeaturedNews = latestFeaturedNews,
            TopNewsByLanguagesCount = topNewsByLanguagesCount,
            LatestTranslations = latestTranslations
        };
    }

    private async Task<NewsComparisonStatisticsDto> BuildComparisonStatisticsAsync(
        NewsStatisticsScope scope,
        CancellationToken cancellationToken
    )
    {
        var comparisonPeriod = GetComparisonPeriod(scope.Request, scope.Today);
        var currentPeriodNewsCount = await ApplyDateRange(
                scope.NewsQueryWithoutDateFilters,
                comparisonPeriod.CurrentFrom,
                comparisonPeriod.CurrentTo
            )
            .CountAsync(cancellationToken);
        var previousPeriodNewsCount = await ApplyDateRange(
                scope.NewsQueryWithoutDateFilters,
                comparisonPeriod.PreviousFrom,
                comparisonPeriod.PreviousTo
            )
            .CountAsync(cancellationToken);

        return new NewsComparisonStatisticsDto
        {
            CurrentPeriodNewsCount = currentPeriodNewsCount,
            PreviousPeriodNewsCount = previousPeriodNewsCount,
            GrowthCount = currentPeriodNewsCount - previousPeriodNewsCount,
            GrowthPercentage = CalculateGrowthPercentage(currentPeriodNewsCount, previousPeriodNewsCount)
        };
    }

    private IQueryable<PerNewsMetricsProjection> BuildPerNewsMetricsQuery(NewsStatisticsScope scope)
    {
        return scope.FilteredNewsQuery.Select(n => new PerNewsMetricsProjection
        {
            NewsId = n.NewsId,
            LanguagesCount = n.NewsUnivTranslations
                .Where(t =>
                    (!scope.HasLanguageFilter || t.LangId == scope.LanguageId)
                    && (
                        !scope.HasSearchFilter
                        || EF.Functions.Like(t.NewsHead ?? string.Empty, scope.SearchPattern)
                        || EF.Functions.Like(t.NewsAbbr ?? string.Empty, scope.SearchPattern)
                        || EF.Functions.Like(t.NewsBody ?? string.Empty, scope.SearchPattern)
                        || EF.Functions.Like(t.NewsSource ?? string.Empty, scope.SearchPattern)
                    )
                    && (
                        !scope.HasSourceFilter
                        || EF.Functions.Like(t.NewsSource ?? string.Empty, scope.SourcePattern)
                    )
                )
                .Select(t => t.LangId)
                .Distinct()
                .Count(),
            TranslationsCount = n.NewsUnivTranslations.Count(t =>
                (!scope.HasLanguageFilter || t.LangId == scope.LanguageId)
                && (
                    !scope.HasSearchFilter
                    || EF.Functions.Like(t.NewsHead ?? string.Empty, scope.SearchPattern)
                    || EF.Functions.Like(t.NewsAbbr ?? string.Empty, scope.SearchPattern)
                    || EF.Functions.Like(t.NewsBody ?? string.Empty, scope.SearchPattern)
                    || EF.Functions.Like(t.NewsSource ?? string.Empty, scope.SearchPattern)
                )
                && (
                    !scope.HasSourceFilter
                    || EF.Functions.Like(t.NewsSource ?? string.Empty, scope.SourcePattern)
                )
            )
        });
    }

    private static NewsStatisticsAppliedFiltersDto BuildAppliedFilters(NewsStatisticsQueryDto request)
    {
        return new NewsStatisticsAppliedFiltersDto
        {
            LanguageId = request.LanguageId,
            Year = request.Year,
            Month = request.Month,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            IsFeatured = request.IsFeatured,
            Search = request.Search,
            Source = request.Source,
            RecentCount = request.RecentCount
        };
    }

    private static IQueryable<NewsUniv> ApplyCommonNewsFilters(
        IQueryable<NewsUniv> query,
        NewsStatisticsQueryDto request
    )
    {
        query = query.Where(n => n.Published);

        if (request.IsFeatured.HasValue)
        {
            query = query.Where(n => n.IsFeatured == request.IsFeatured.Value);
        }

        if (request.LanguageId.HasValue)
        {
            query = query.Where(n => n.NewsUnivTranslations.Any(t => t.LangId == request.LanguageId.Value));
        }

        if (request.Search.HasValue())
        {
            var searchPattern = $"%{request.Search.Trim()}%";
            query = query.Where(n =>
                n.NewsUnivTranslations.Any(t =>
                    EF.Functions.Like(t.NewsHead ?? string.Empty, searchPattern)
                    || EF.Functions.Like(t.NewsAbbr ?? string.Empty, searchPattern)
                    || EF.Functions.Like(t.NewsBody ?? string.Empty, searchPattern)
                    || EF.Functions.Like(t.NewsSource ?? string.Empty, searchPattern)
                )
            );
        }

        if (request.Source.HasValue())
        {
            var sourcePattern = $"%{request.Source.Trim()}%";
            query = query.Where(n =>
                n.NewsUnivTranslations.Any(t => EF.Functions.Like(t.NewsSource ?? string.Empty, sourcePattern))
            );
        }

        return query;
    }

    private static IQueryable<NewsUniv> ApplyDateFilters(
        IQueryable<NewsUniv> query,
        NewsStatisticsQueryDto request
    )
    {
        if (request.FromDate.HasValue || request.ToDate.HasValue)
        {
            if (request.FromDate.HasValue)
            {
                var fromDate = request.FromDate.Value.Date;
                query = query.Where(n => n.NewsDate.HasValue && n.NewsDate.Value.Date >= fromDate);
            }

            if (request.ToDate.HasValue)
            {
                var toDate = request.ToDate.Value.Date;
                query = query.Where(n => n.NewsDate.HasValue && n.NewsDate.Value.Date <= toDate);
            }

            return query;
        }

        if (request.Year.HasValue && request.Month.HasValue)
        {
            query = query.Where(n =>
                n.NewsDate.HasValue
                && n.NewsDate.Value.Year == request.Year.Value
                && n.NewsDate.Value.Month == request.Month.Value
            );
        }
        else if (request.Year.HasValue)
        {
            query = query.Where(n => n.NewsDate.HasValue && n.NewsDate.Value.Year == request.Year.Value);
        }

        return query;
    }

    private static IQueryable<NewsUnivTranslation> ApplyCommonTranslationFilters(
        IQueryable<NewsUnivTranslation> query,
        NewsStatisticsQueryDto request
    )
    {
        if (request.LanguageId.HasValue)
        {
            query = query.Where(t => t.LangId == request.LanguageId.Value);
        }

        if (request.Search.HasValue())
        {
            var searchPattern = $"%{request.Search.Trim()}%";
            query = query.Where(t =>
                EF.Functions.Like(t.NewsHead ?? string.Empty, searchPattern)
                || EF.Functions.Like(t.NewsAbbr ?? string.Empty, searchPattern)
                || EF.Functions.Like(t.NewsBody ?? string.Empty, searchPattern)
                || EF.Functions.Like(t.NewsSource ?? string.Empty, searchPattern)
            );
        }

        if (request.Source.HasValue())
        {
            var sourcePattern = $"%{request.Source.Trim()}%";
            query = query.Where(t => EF.Functions.Like(t.NewsSource ?? string.Empty, sourcePattern));
        }

        return query;
    }

    private static IQueryable<NewsStatisticsDayCountDto> BuildDaySeriesQuery(
        IQueryable<NewsUniv> query,
        NewsStatisticsQueryDto request
    )
    {
        if (request.Year.HasValue && request.Month.HasValue)
        {
            return query
                .GroupBy(n => n.NewsDate!.Value.Date)
                .Select(g => new NewsStatisticsDayCountDto
                {
                    Date = g.Key,
                    NewsCount = g.Count()
                })
                .OrderBy(x => x.Date);
        }

        if (request.FromDate.HasValue || request.ToDate.HasValue)
        {
            return query
                .GroupBy(n => n.NewsDate!.Value.Date)
                .Select(g => new NewsStatisticsDayCountDto
                {
                    Date = g.Key,
                    NewsCount = g.Count()
                })
                .OrderBy(x => x.Date);
        }

        return query
            .GroupBy(n => n.NewsDate!.Value.Date)
            .Select(g => new NewsStatisticsDayCountDto
            {
                Date = g.Key,
                NewsCount = g.Count()
            })
            .OrderBy(x => x.Date);
    }

    private static IQueryable<RecentNewsProjection> BuildLatestNewsQuery(
        IQueryable<NewsUniv> query,
        NewsStatisticsScope scope
    )
    {
        return query.Select(n => new RecentNewsProjection
        {
            Id = n.NewsId,
            Date = n.NewsDate,
            IsFeatured = n.IsFeatured,
            OwnerId = n.OwnerId,
            NewsImg = n.NewsImg,
            Title = n.NewsUnivTranslations
                .Where(t =>
                    (!scope.HasLanguageFilter || t.LangId == scope.LanguageId)
                    && (
                        !scope.HasSearchFilter
                        || EF.Functions.Like(t.NewsHead ?? string.Empty, scope.SearchPattern)
                        || EF.Functions.Like(t.NewsAbbr ?? string.Empty, scope.SearchPattern)
                        || EF.Functions.Like(t.NewsBody ?? string.Empty, scope.SearchPattern)
                        || EF.Functions.Like(t.NewsSource ?? string.Empty, scope.SearchPattern)
                    )
                    && (
                        !scope.HasSourceFilter
                        || EF.Functions.Like(t.NewsSource ?? string.Empty, scope.SourcePattern)
                    )
                )
                .OrderBy(t => t.Id)
                .Select(t => t.NewsHead)
                .FirstOrDefault() ?? string.Empty,
            LanguageId = n.NewsUnivTranslations
                .Where(t =>
                    (!scope.HasLanguageFilter || t.LangId == scope.LanguageId)
                    && (
                        !scope.HasSearchFilter
                        || EF.Functions.Like(t.NewsHead ?? string.Empty, scope.SearchPattern)
                        || EF.Functions.Like(t.NewsAbbr ?? string.Empty, scope.SearchPattern)
                        || EF.Functions.Like(t.NewsBody ?? string.Empty, scope.SearchPattern)
                        || EF.Functions.Like(t.NewsSource ?? string.Empty, scope.SearchPattern)
                    )
                    && (
                        !scope.HasSourceFilter
                        || EF.Functions.Like(t.NewsSource ?? string.Empty, scope.SourcePattern)
                    )
                )
                .OrderBy(t => t.Id)
                .Select(t => (int?)t.LangId)
                .FirstOrDefault(),
            LanguagesCount = n.NewsUnivTranslations
                .Where(t =>
                    (!scope.HasLanguageFilter || t.LangId == scope.LanguageId)
                    && (
                        !scope.HasSearchFilter
                        || EF.Functions.Like(t.NewsHead ?? string.Empty, scope.SearchPattern)
                        || EF.Functions.Like(t.NewsAbbr ?? string.Empty, scope.SearchPattern)
                        || EF.Functions.Like(t.NewsBody ?? string.Empty, scope.SearchPattern)
                        || EF.Functions.Like(t.NewsSource ?? string.Empty, scope.SearchPattern)
                    )
                    && (
                        !scope.HasSourceFilter
                        || EF.Functions.Like(t.NewsSource ?? string.Empty, scope.SourcePattern)
                    )
                )
                .Select(t => t.LangId)
                .Distinct()
                .Count()
        });
    }

    private static List<NewsStatisticsRecentNewsItemDto> FinalizeRecentNewsItems(
        IEnumerable<RecentNewsProjection> items
    )
    {
        return items
            .Select(x => new NewsStatisticsRecentNewsItemDto
            {
                Id = x.Id,
                Date = x.Date,
                IsFeatured = x.IsFeatured,
                NewsImg = StringExtensions.GetFullPath(
                    x.OwnerId,
                    x.NewsImg,
                    UniversityNewsListingImageBasePath
                ),
                Title = StringExtensions.StripHtml(x.Title),
                LanguageId = x.LanguageId,
                LanguagesCount = x.LanguagesCount
            })
            .ToList();
    }

    private static List<NewsStatisticsRecentTranslationItemDto> FinalizeRecentTranslationItems(
        IEnumerable<RecentTranslationProjection> items,
        IReadOnlyDictionary<int, LanguageDto> languageLookup
    )
    {
        return items
            .Select(x =>
            {
                var fallbackLanguage = ResolveLanguage(x.LanguageId, languageLookup);
                return new NewsStatisticsRecentTranslationItemDto
                {
                    TranslationId = x.TranslationId,
                    NewsId = x.NewsId,
                    Date = x.Date,
                    IsFeatured = x.IsFeatured,
                    LanguageId = x.LanguageId,
                    LanguageCode = x.LanguageCode,
                    LanguageName = x.LanguageCode.HasValue()
                        ? ResolveLanguageName(x.LanguageCode)
                        : fallbackLanguage.Name,
                    Title = StringExtensions.StripHtml(x.Title),
                    Source = StringExtensions.StripHtml(x.Source)
                };
            })
            .ToList();
    }

    private static IQueryable<NewsUniv> ApplyDateRange(
        IQueryable<NewsUniv> query,
        DateTime from,
        DateTime to
    )
    {
        var start = from.Date;
        var end = to.Date;
        return query.Where(n => n.NewsDate.HasValue && n.NewsDate.Value.Date >= start && n.NewsDate.Value.Date <= end);
    }

    private static (DateTime CurrentFrom, DateTime CurrentTo, DateTime PreviousFrom, DateTime PreviousTo) GetComparisonPeriod(
        NewsStatisticsQueryDto request,
        DateTime today
    )
    {
        if (request.FromDate.HasValue || request.ToDate.HasValue)
        {
            var currentFrom = (request.FromDate ?? request.ToDate ?? today).Date;
            var currentTo = (request.ToDate ?? request.FromDate ?? today).Date;
            var duration = (currentTo - currentFrom).Days + 1;
            var previousTo = currentFrom.AddDays(-1);
            var previousFrom = previousTo.AddDays(-(duration - 1));
            return (currentFrom, currentTo, previousFrom, previousTo);
        }

        if (request.Year.HasValue && request.Month.HasValue)
        {
            var current = GetMonthPeriod(request.Year.Value, request.Month.Value);
            var previousAnchor = current.From.AddMonths(-1);
            var previous = GetMonthPeriod(previousAnchor.Year, previousAnchor.Month);
            return (current.From, current.To, previous.From, previous.To);
        }

        if (request.Year.HasValue)
        {
            var currentFrom = new DateTime(request.Year.Value, 1, 1);
            var currentTo = new DateTime(request.Year.Value, 12, 31);
            var previousFrom = new DateTime(request.Year.Value - 1, 1, 1);
            var previousTo = new DateTime(request.Year.Value - 1, 12, 31);
            return (currentFrom, currentTo, previousFrom, previousTo);
        }

        var defaultCurrentTo = today.Date;
        var defaultCurrentFrom = defaultCurrentTo.AddDays(-29);
        var defaultPreviousTo = defaultCurrentFrom.AddDays(-1);
        var defaultPreviousFrom = defaultPreviousTo.AddDays(-29);
        return (defaultCurrentFrom, defaultCurrentTo, defaultPreviousFrom, defaultPreviousTo);
    }

    private static (DateTime From, DateTime To) GetMonthPeriod(int year, int month)
    {
        var from = new DateTime(year, month, 1);
        return (from, from.AddMonths(1).AddDays(-1));
    }

    private static Dictionary<int, LanguageDto> BuildLanguageLookup(IEnumerable<Language> languages)
    {
        return languages.ToDictionary(x => x.Id, ToLanguageDto);
    }

    private static LanguageDto ResolveLanguage(int languageId, IReadOnlyDictionary<int, LanguageDto> lookup)
    {
        if (lookup.TryGetValue(languageId, out var language))
        {
            return language;
        }

        return new LanguageDto
        {
            Id = languageId,
            Code = string.Empty,
            Name = $"Language {languageId}",
            Flag = string.Empty
        };
    }

    private static LanguageDto ToLanguageDto(Language language)
    {
        return new LanguageDto
        {
            Id = language.Id,
            Code = language.LCID,
            Name = ResolveLanguageName(language.LCID),
            Flag = ResolveLanguageFlag(language.LCID)
        };
    }

    private static string ResolveLanguageName(string code)
    {
        var language = StaticLanguages.LanguageModels.FirstOrDefault(x =>
            string.Equals(x.Code?.Trim(), code?.Trim(), StringComparison.OrdinalIgnoreCase)
        );
        return language?.Name ?? code ?? string.Empty;
    }

    private static string ResolveLanguageFlag(string code)
    {
        var language = StaticLanguages.LanguageModels.FirstOrDefault(x =>
            string.Equals(x.Code?.Trim(), code?.Trim(), StringComparison.OrdinalIgnoreCase)
        );
        return language?.Flag ?? string.Empty;
    }

    private static string ValidateRequest(NewsStatisticsQueryDto request)
    {
        var currentYear = DateTime.Today.Year + 1;

        if (request.Month.HasValue && (request.Month.Value < 1 || request.Month.Value > 12))
        {
            return "Month must be between 1 and 12.";
        }

        if (request.Year.HasValue && (request.Year.Value <= 1900 || request.Year.Value > currentYear))
        {
            return $"Year must be greater than 1900 and less than or equal to {currentYear}.";
        }

        if (request.Month.HasValue && !request.Year.HasValue)
        {
            return "Month filter requires Year.";
        }

        if (
            request.FromDate.HasValue
            && request.ToDate.HasValue
            && request.FromDate.Value.Date > request.ToDate.Value.Date
        )
        {
            return "FromDate cannot be after ToDate.";
        }

        if (request.RecentCount < 1 || request.RecentCount > 50)
        {
            return "RecentCount must be between 1 and 50.";
        }

        return string.Empty;
    }

    private static double RoundValue(double value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    private static double RoundPercentage(double value)
    {
        return RoundValue(value);
    }

    private static double CalculateGrowthPercentage(int current, int previous)
    {
        if (previous == 0)
        {
            return current == 0 ? 0 : 100;
        }

        return RoundPercentage(((double)(current - previous) / previous) * 100);
    }

    private sealed class NewsStatisticsScope
    {
        public NewsStatisticsQueryDto Request { get; set; }
        public DateTime Today { get; set; }
        public bool HasLanguageFilter { get; set; }
        public int LanguageId { get; set; }
        public bool HasSearchFilter { get; set; }
        public string SearchPattern { get; set; }
        public bool HasSourceFilter { get; set; }
        public string SourcePattern { get; set; }
        public IQueryable<NewsUniv> FilteredNewsQuery { get; set; }
        public IQueryable<NewsUniv> NewsQueryWithoutDateFilters { get; set; }
        public IQueryable<NewsUnivTranslation> FilteredTranslationsQuery { get; set; }
    }

    private sealed class PerNewsMetricsProjection
    {
        public int NewsId { get; set; }
        public int LanguagesCount { get; set; }
        public int TranslationsCount { get; set; }
    }

    private sealed class RecentNewsProjection
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public bool IsFeatured { get; set; }
        public Guid OwnerId { get; set; }
        public string NewsImg { get; set; }
        public string Title { get; set; }
        public int? LanguageId { get; set; }
        public int LanguagesCount { get; set; }
    }

    private sealed class RecentTranslationProjection
    {
        public int TranslationId { get; set; }
        public int NewsId { get; set; }
        public DateTime? Date { get; set; }
        public bool IsFeatured { get; set; }
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; }
        public string LanguageName { get; set; }
        public string Title { get; set; }
        public string Source { get; set; }
    }
}
