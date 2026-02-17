namespace TheBoys.Application.Common.Requests;

public class PaginateNewsRequest : PaginateRequest
{
    public int LanguageId { get; set; } = 2;
}
