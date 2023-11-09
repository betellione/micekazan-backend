using System.Text.Json.Serialization;

namespace WebApp1.External.Qtickets.Contracts.Responses;

public class Ticket
{
    [JsonPropertyName("pdf_url")]
    public string PdfUri { get; set; } = null!;
    [JsonPropertyName("pdf_url")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("pdf_url")]
    public string Surname { get; set; } = null!;
    [JsonPropertyName("pdf_url")]
    public string Patronymic { get; set; } = null!;
}