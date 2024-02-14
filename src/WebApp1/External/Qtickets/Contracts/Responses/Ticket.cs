using System.Text.Json.Serialization;

namespace WebApp1.External.Qtickets.Contracts.Responses;

public class Ticket
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("barcode")]
    public string Barcode { get; set; } = null!;

    [JsonPropertyName("client_email")]
    public string ClientEmail { get; set; } = null!;
    
    [JsonPropertyName("show_id")]
    public long ShowId { get; set; }
}