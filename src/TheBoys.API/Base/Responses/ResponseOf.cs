using System.Text.Json.Serialization;

namespace TheBoys.API.Bases.Responses;

public record ResponseOf<TResponse> : Response
{
    [JsonPropertyOrder(11)]
    public TResponse Result { get; set; }
}
