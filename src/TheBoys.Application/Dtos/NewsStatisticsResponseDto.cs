namespace TheBoys.Application.Dtos;

public class NewsStatisticsResponseDto
{
    public NewsStatisticsAppliedFiltersDto Filters { get; set; } = new();
    public NewsOverviewStatisticsDto Overview { get; set; } = new();
    public NewsLanguageStatisticsDto Languages { get; set; } = new();
    public NewsTimeStatisticsDto Time { get; set; } = new();
    public NewsFeaturedStatisticsDto Featured { get; set; } = new();
    public NewsContentCompletenessStatisticsDto ContentCompleteness { get; set; } = new();
    public NewsSourceStatisticsDto Sources { get; set; } = new();
    public NewsRecentStatisticsDto Recent { get; set; } = new();
    public NewsComparisonStatisticsDto Comparisons { get; set; } = new();
}

public class NewsOverviewStatisticsResponseDto
{
    public NewsStatisticsAppliedFiltersDto Filters { get; set; } = new();
    public NewsOverviewStatisticsDto Overview { get; set; } = new();
}

public class NewsLanguageStatisticsResponseDto
{
    public NewsStatisticsAppliedFiltersDto Filters { get; set; } = new();
    public NewsLanguageStatisticsDto Languages { get; set; } = new();
}

public class NewsTimeStatisticsResponseDto
{
    public NewsStatisticsAppliedFiltersDto Filters { get; set; } = new();
    public NewsTimeStatisticsDto Time { get; set; } = new();
}

public class NewsFeaturedStatisticsResponseDto
{
    public NewsStatisticsAppliedFiltersDto Filters { get; set; } = new();
    public NewsFeaturedStatisticsDto Featured { get; set; } = new();
}

public class NewsContentCompletenessStatisticsResponseDto
{
    public NewsStatisticsAppliedFiltersDto Filters { get; set; } = new();
    public NewsContentCompletenessStatisticsDto ContentCompleteness { get; set; } = new();
}

public class NewsSourceStatisticsResponseDto
{
    public NewsStatisticsAppliedFiltersDto Filters { get; set; } = new();
    public NewsSourceStatisticsDto Sources { get; set; } = new();
}

public class NewsRecentStatisticsResponseDto
{
    public NewsStatisticsAppliedFiltersDto Filters { get; set; } = new();
    public NewsRecentStatisticsDto Recent { get; set; } = new();
}

public class NewsComparisonStatisticsResponseDto
{
    public NewsStatisticsAppliedFiltersDto Filters { get; set; } = new();
    public NewsComparisonStatisticsDto Comparisons { get; set; } = new();
}

public class NewsStatisticsAppliedFiltersDto
{
    public int? LanguageId { get; set; }
    public int? Year { get; set; }
    public int? Month { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool? IsFeatured { get; set; }
    public string Search { get; set; }
    public string Source { get; set; }
    public int RecentCount { get; set; }
}

public class NewsOverviewStatisticsDto
{
    public int TotalNewsCount { get; set; }
    public int TotalTranslationsCount { get; set; }
    public int TotalLanguagesCount { get; set; }
    public int FeaturedNewsCount { get; set; }
    public int NotFeaturedNewsCount { get; set; }
    public int NewsWithImageCount { get; set; }
    public int NewsWithoutImageCount { get; set; }
    public double AverageTranslationsPerNews { get; set; }
    public int NewsWithSingleLanguageCount { get; set; }
    public int NewsWithMultipleLanguagesCount { get; set; }
    public int MaxLanguagesForSingleNews { get; set; }
    public int MinLanguagesForSingleNews { get; set; }
}

public class NewsLanguageStatisticsDto
{
    public List<NewsStatisticsLanguageItemDto> NewsCountByLanguage { get; set; } = new();
    public List<NewsStatisticsLanguageItemDto> TranslationsCountByLanguage { get; set; } = new();
    public List<NewsStatisticsLanguagePercentageDto> PercentageByLanguage { get; set; } = new();
    public NewsStatisticsLanguageItemDto MostUsedLanguage { get; set; }
    public NewsStatisticsLanguageItemDto LeastUsedLanguage { get; set; }
    public List<LanguageDto> LanguagesWithNoNews { get; set; } = new();
    public List<NewsStatisticsDistributionItemDto> LanguageDistributionPerNews { get; set; } = new();
}

public class NewsTimeStatisticsDto
{
    public List<NewsStatisticsYearCountDto> NewsCountByYear { get; set; } = new();
    public List<NewsStatisticsMonthCountDto> NewsCountByMonth { get; set; } = new();
    public List<NewsStatisticsDayCountDto> NewsCountByDay { get; set; } = new();
    public int NewsCountLast7Days { get; set; }
    public int NewsCountLast30Days { get; set; }
    public int NewsCountThisMonth { get; set; }
    public int NewsCountThisYear { get; set; }
    public NewsStatisticsDayCountDto BusiestDay { get; set; }
    public NewsStatisticsMonthCountDto BusiestMonth { get; set; }
    public double MonthlyGrowthPercentage { get; set; }
    public double YearlyGrowthPercentage { get; set; }
}

public class NewsFeaturedStatisticsDto
{
    public int FeaturedCount { get; set; }
    public int NonFeaturedCount { get; set; }
    public double FeaturedPercentage { get; set; }
    public List<NewsStatisticsMonthCountDto> FeaturedByMonth { get; set; } = new();
    public List<NewsStatisticsRecentNewsItemDto> LatestFeaturedNews { get; set; } = new();
}

public class NewsContentCompletenessStatisticsDto
{
    public int NewsWithImageCount { get; set; }
    public int NewsWithoutImageCount { get; set; }
    public int TranslationsWithHeadCount { get; set; }
    public int TranslationsMissingHeadCount { get; set; }
    public int TranslationsWithAbbrCount { get; set; }
    public int TranslationsMissingAbbrCount { get; set; }
    public int TranslationsWithBodyCount { get; set; }
    public int TranslationsMissingBodyCount { get; set; }
    public int TranslationsWithSourceCount { get; set; }
    public int TranslationsMissingSourceCount { get; set; }
    public int TranslationsWithImgAltCount { get; set; }
    public int TranslationsMissingImgAltCount { get; set; }
    public double CompletionRatePercentage { get; set; }
}

public class NewsSourceStatisticsDto
{
    public List<NewsStatisticsSourceItemDto> NewsCountBySource { get; set; } = new();
    public List<NewsStatisticsSourceItemDto> TranslationsCountBySource { get; set; } = new();
    public NewsStatisticsSourceItemDto MostCommonSource { get; set; }
    public int MissingSourceCount { get; set; }
}

public class NewsRecentStatisticsDto
{
    public List<NewsStatisticsRecentNewsItemDto> LatestNews { get; set; } = new();
    public List<NewsStatisticsRecentNewsItemDto> LatestFeaturedNews { get; set; } = new();
    public List<NewsStatisticsRecentNewsItemDto> TopNewsByLanguagesCount { get; set; } = new();
    public List<NewsStatisticsRecentTranslationItemDto> LatestTranslations { get; set; } = new();
}

public class NewsComparisonStatisticsDto
{
    public int CurrentPeriodNewsCount { get; set; }
    public int PreviousPeriodNewsCount { get; set; }
    public int GrowthCount { get; set; }
    public double GrowthPercentage { get; set; }
}

public class NewsStatisticsLanguageItemDto
{
    public int LanguageId { get; set; }
    public string LanguageName { get; set; }
    public string LanguageCode { get; set; }
    public int NewsCount { get; set; }
    public int TranslationsCount { get; set; }
    public double Percentage { get; set; }
}

public class NewsStatisticsLanguagePercentageDto
{
    public int LanguageId { get; set; }
    public string LanguageName { get; set; }
    public string LanguageCode { get; set; }
    public double Percentage { get; set; }
}

public class NewsStatisticsDistributionItemDto
{
    public int LanguagesCount { get; set; }
    public int NewsCount { get; set; }
}

public class NewsStatisticsYearCountDto
{
    public int Year { get; set; }
    public int NewsCount { get; set; }
}

public class NewsStatisticsMonthCountDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string Label { get; set; }
    public int NewsCount { get; set; }
}

public class NewsStatisticsDayCountDto
{
    public DateTime Date { get; set; }
    public int NewsCount { get; set; }
}

public class NewsStatisticsSourceItemDto
{
    public string Source { get; set; }
    public int NewsCount { get; set; }
    public int TranslationsCount { get; set; }
}

public class NewsStatisticsRecentNewsItemDto
{
    public int Id { get; set; }
    public DateTime? Date { get; set; }
    public bool IsFeatured { get; set; }
    public string NewsImg { get; set; }
    public string Title { get; set; }
    public int? LanguageId { get; set; }
    public int LanguagesCount { get; set; }
}

public class NewsStatisticsRecentTranslationItemDto
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
