using System.Text.Json.Serialization;

namespace WebApp1.External.SmsRu.Contracts.Responses;

public class PhoneCallResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    [JsonPropertyName("code")]
    public int Code { get; set; }
}