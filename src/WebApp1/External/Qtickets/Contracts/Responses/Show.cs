using System.Text.Json.Serialization;

namespace WebApp1.External.Qtickets.Contracts.Responses;

public class Show
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("start_date")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("finish_date")]
    public DateTime FinishDate { get; set; }
}