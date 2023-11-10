using System.Text.Json.Serialization;

namespace WebApp1.External.SmsRu.Contracts.Responses;

public class SendSms
{
    [JsonPropertyName("sms")]
    public Dictionary<string, SmsEntry> Sms { get; set; } = null!;
}

public record SmsEntry(string Status, int StatusCode, string? SmsId, string? Cost, int? Sms);