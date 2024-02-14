using System.Text.Json.Serialization;

namespace WebApp1.External.SmsRu.Contracts.Responses;

public class SendSms
{
    [JsonPropertyName("sms")]
    public Dictionary<string, SmsEntry> Sms { get; set; } = null!;
}

public class SmsEntry
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("sms_id")]
    public string? SmsId { get; set; }

    [JsonPropertyName("cost")]
    public decimal? Cost { get; set; }

    [JsonPropertyName("sms")]
    public int? Sms { get; set; }
}