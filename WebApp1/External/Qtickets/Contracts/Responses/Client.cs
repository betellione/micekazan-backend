// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ClassNeverInstantiated.Local

using System.Text.Json.Serialization;

namespace WebApp1.External.Qtickets.Contracts.Responses;

public class Client
{
    [JsonPropertyName("details")]
    private Details _details = null!;

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonIgnore]
    public string? Name => _details.Name;

    [JsonIgnore]
    public string? Surname => _details.Surname;

    [JsonIgnore]
    public string? Patronymic => _details.Patronymic;

    [JsonIgnore]
    public string? PhoneNumber => _details.PhoneNumber;

    private class Details
    {
        [JsonPropertyName("name")]
        public string? Name { get; }

        [JsonPropertyName("surname")]
        public string? Surname { get; }

        [JsonPropertyName("middlename")]
        public string? Patronymic { get; }

        [JsonPropertyName("phone")]
        public string? PhoneNumber { get; }
    }
}