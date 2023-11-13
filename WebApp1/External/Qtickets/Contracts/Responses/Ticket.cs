using System.Text.Json.Serialization;

namespace WebApp1.External.Qtickets.Contracts.Responses;

public class Ticket
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("barcode")]
    public string Barcode { get; set; } = null!;
    
    [JsonPropertyName("client_email")]
    public long ClientEmail { get; set; }
    
    [JsonPropertyName("show_id")]
    public long ShowId { get; set; }
}