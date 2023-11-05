using System.Text.Json.Serialization;

namespace WebApp1.External.Qtickets.Contracts.Responses;

public class Ticket
{
    [JsonPropertyName("pdf_url")]
    public string PdfUri { get; set; } = null!;
}