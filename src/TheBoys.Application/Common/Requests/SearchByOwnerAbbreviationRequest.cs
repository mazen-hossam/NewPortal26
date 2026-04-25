namespace TheBoys.Application.Common.Requests;

public class SearchByOwnerAbbreviationRequest : PaginateRequest
{
    public string Abbreviation { get; set; } = string.Empty;
    public int Lid { get; set; } = 2;
}
