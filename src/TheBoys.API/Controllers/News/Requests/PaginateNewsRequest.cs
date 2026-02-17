using TheBoys.API.Base.Requests;

namespace TheBoys.API.Controllers.News.Requests;

public class PaginateNewsRequest : PaginateRequest
{
    public int LanguageId { get; set; } = 2;
}
