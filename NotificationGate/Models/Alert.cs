using System.Text.Json.Serialization;

namespace NotificationGate.Models;

public class Alert
{
    [JsonPropertyName("alert_id")]
    public string? Alertid { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("priority")]
    public string? Priority { get; set; }

    [JsonPropertyName("classification")]
    public string? Classification { get; set; }

    [JsonPropertyName("lat")]
    public string? lat { get; set; }

    [JsonPropertyName("lon")]
    public string? Lon { get; set; }

    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

