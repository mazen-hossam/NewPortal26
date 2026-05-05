using TheBoys.Domain.Entities;

namespace TheBoys.Application.Dtos;

public sealed class FacultyNewsDetailsDto : FacultyNewsItemDto
{
    public string Body { get; set; } = string.Empty;
    public List<LanguageModel> Languages { get; set; } = new List<LanguageModel>();
}
