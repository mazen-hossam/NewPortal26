using TheBoys.API.Entities;

namespace TheBoys.API.Dtos;

public class NewsDto
{
    public int Id { get; set; }
    public DateTime? Date { get; set; }
    public bool IsFeatured { get; set; }
    public string NewsImg { get; set; }
    public NewsTranslationDto NewsDetails { get; set; }
    public List<LanguageModel> Languages { get; set; }
}
