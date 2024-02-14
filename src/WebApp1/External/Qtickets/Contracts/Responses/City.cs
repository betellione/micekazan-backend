using System.Text.Json.Serialization;

namespace WebApp1.External.Qtickets.Contracts.Responses;

public class City
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}