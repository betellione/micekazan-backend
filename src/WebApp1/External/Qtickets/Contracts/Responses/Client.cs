// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ClassNeverInstantiated.Local

using System.Text.Json.Serialization;

namespace WebApp1.External.Qtickets.Contracts.Responses;

public class Client
{
    [JsonPropertyName("details")]
    public DetailsEntry Details { get; set; } = null!;

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonIgnore]
    public string? Name => Details.Name;

    [JsonIgnore]
    public string? Surname => Details.Surname;

    [JsonIgnore]
    public string? Patronymic => Details.Patronymic;

    [JsonIgnore]
    public string? PhoneNumber => Details.PhoneNumber;

    public class DetailsEntry
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("surname")]
        public string? Surname { get; set; }

        [JsonPropertyName("middlename")]
        public string? Patronymic { get; set; }

        [JsonPropertyName("phone")]
        public string? PhoneNumber { get; set; }
    }
}