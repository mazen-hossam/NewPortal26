namespace TheBoys.Application.Dtos;

public class NewsStatisticsQueryDto
{
    public int? LanguageId { get; set; }
    public int? Year { get; set; }
    public int? Month { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool? IsFeatured { get; set; }
    public string Search { get; set; }
    public string Source { get; set; }
    public int RecentCount { get; set; } = 10;
}
