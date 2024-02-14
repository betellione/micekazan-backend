using System.Text.Json.Serialization;

namespace WebApp1.External.Qtickets.Contracts.Responses;

public class ClientData
{
    [JsonPropertyName("client_email")]
    public string? ClientEmail { get; set; }

    [JsonPropertyName("client_phone")]
    public string? ClientPhone { get; set; }

    [JsonPropertyName("client_name")]
    public string? ClientName { get; set; }

    [JsonPropertyName("client_surname")]
    public string? ClientSurname { get; set; }

    [JsonPropertyName("work_position")]
    public string? WorkPosition { get; set; }

    [JsonPropertyName("client_middlename")]
    public string? ClientMiddlename { get; set; }

    [JsonPropertyName("organization_name")]
    public string? OrganizationName { get; set; }
}