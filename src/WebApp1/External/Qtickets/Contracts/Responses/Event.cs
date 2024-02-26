using System.Text.Json.Serialization;

namespace WebApp1.External.Qtickets.Contracts.Responses;

public class Event
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("city")]
    public City City { get; set; } = null!;

    [JsonPropertyName("shows")]
    public IEnumerable<Show> Shows { get; set; } = Enumerable.Empty<Show>();

    [JsonIgnore]
    public IEnumerable<long> ShowIds { get; set; } = Enumerable.Empty<long>();
}