namespace TheBoys.Application.Dtos;

public class FacultyNewsItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public DateTime? CurrentDate { get; set; }
    public string Image { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string ImageAlt { get; set; } = string.Empty;
}
