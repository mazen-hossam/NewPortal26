using System.Text.Json.Serialization;

namespace TheBoys.API.Bases.Responses;

public sealed record PaginationResponse<TResponse> : ResponseOf<TResponse>
{
    [JsonPropertyOrder(4)]
    public int PageSize { get; set; }

    [JsonPropertyOrder(5)]
    public int PageIndex { get; set; }

    [JsonPropertyOrder(6)]
    public int TotalCount { get; set; }

    [JsonPropertyOrder(7)]
    public int Count { get; set; }

    [JsonPropertyOrder(8)]
    public int TotalPages =>
        Convert.ToInt32(Math.Ceiling(TotalCount / Convert.ToDecimal(PageSize)));

    [JsonPropertyOrder(9)]
    public bool MoveNext => PageIndex < TotalPages;

    [JsonPropertyOrder(10)]
    public bool MovePrevious => PageIndex > 1;
}
