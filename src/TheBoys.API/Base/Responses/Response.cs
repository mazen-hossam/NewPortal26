using System.Net;
using System.Text.Json.Serialization;

namespace TheBoys.API.Bases.Responses;

public record Response
{
    public Response() => StatusCode = (int)HttpStatusCode.OK;

    [JsonPropertyOrder(1)]
    public bool Success { get; set; }

    [JsonPropertyOrder(2)]
    public int StatusCode { get; set; }

    [JsonPropertyOrder(3)]
    public string Message { get; set; }

    public void SendSuccess(string message = "operation done successfully.")
    {
        Message = message;
        Success = true;
        StatusCode = (int)HttpStatusCode.OK;
    }

    public void SendBadRequest(string message = "error")
    {
        Success = false;
        StatusCode = (int)HttpStatusCode.BadRequest;
        Message = message;
    }
}
