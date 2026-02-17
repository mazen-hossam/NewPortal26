using System.Text.Json.Serialization;

namespace TheBoys.Application.Common.Responses;

public record ResponseOf<TResponse> : Response
{
    [JsonPropertyOrder(11)]
    public TResponse Result { get; set; }
}
