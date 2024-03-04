using System.Text.Json.Serialization;

namespace WebApp1.External.Qtickets.Contracts.Responses;

public class Order
{
    [JsonPropertyName("client_id")]
    public long ClientId { get; set; }

    [JsonPropertyName("event_id")]
    public long EventId { get; set; }

    [JsonPropertyName("baskets")]
    public IEnumerable<Ticket> Baskets { get; set; } = Enumerable.Empty<Ticket>();
}