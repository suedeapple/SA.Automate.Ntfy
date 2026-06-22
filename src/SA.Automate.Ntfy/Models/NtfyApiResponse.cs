using System.Text.Json.Serialization;

namespace SA.Automate.Ntfy.Models;

/// <summary>
/// Represents a successful publish response from the ntfy API.
/// </summary>
internal class NtfyApiResponse
{
    /// <summary>
    /// Gets the randomly chosen message identifier assigned by ntfy.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets the unix timestamp of when the message was created.
    /// </summary>
    [JsonPropertyName("time")]
    public long Time { get; set; }

    /// <summary>
    /// Gets the topic the message was published to.
    /// </summary>
    [JsonPropertyName("topic")]
    public string? Topic { get; set; }

    /// <summary>
    /// Gets the notification title, if one was set.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets the notification message body.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Gets the tags attached to the notification, if any.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Gets the priority the notification was sent with.
    /// </summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }
}
