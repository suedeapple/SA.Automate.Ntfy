using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using SA.Automate.Ntfy.Actions;
using SA.Automate.Ntfy.Models;
using Umbraco.Automate.Core.Connections;

namespace SA.Automate.Ntfy.Http;

/// <summary>
/// Shared request-building logic for talking to the ntfy API, used by both the public and
/// authenticated connection types and actions so the JSON shape and auth header handling
/// only need to be defined once.
/// </summary>
internal static class NtfyRequestHelper
{
    /// <summary>
    /// Resolves the effective ntfy server URL: a per-connection override wins, then the
    /// globally configured default, then ntfy's public server as a last resort.
    /// </summary>
    public static string ResolveServerUrl(string? connectionOverride, string globalDefault)
    {
        var url = !string.IsNullOrWhiteSpace(connectionOverride)
            ? connectionOverride
            : (!string.IsNullOrWhiteSpace(globalDefault) ? globalDefault : "https://ntfy.sh");

        return url.TrimEnd('/');
    }

    /// <summary>
    /// Resolves the effective access token: a per-connection token wins, then the globally configured
    /// default if the connection has opted in to it, otherwise no token (e.g. for public topics).
    /// </summary>
    public static string? ResolveAccessToken(string? connectionOverride, bool useDefaultAccessToken, string? globalDefault)
    {
        if (!string.IsNullOrWhiteSpace(connectionOverride))
            return connectionOverride;

        return useDefaultAccessToken ? globalDefault : null;
    }

    /// <summary>
    /// Performs a lightweight reachability check against a topic without publishing a visible
    /// notification, used to power the back office "Test connection" action.
    /// Note: this checks read access, which on strictly ACL'd self-hosted servers can differ
    /// from the write/publish access actually needed by the action.
    /// </summary>
    public static async Task<ConnectionValidationResult> ValidateTopicAsync(
        HttpClient client,
        string serverUrl,
        string topic,
        string? accessToken,
        CancellationToken cancellationToken)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{serverUrl}/{topic}/json?poll=1&limit=1");
            ApplyAuthHeader(request.Headers, accessToken);

            using var response = await client.SendAsync(request, cancellationToken);

            return response.IsSuccessStatusCode
                ? ConnectionValidationResult.Success("Connected")
                : ConnectionValidationResult.Failure(
                    $"ntfy rejected the request with status {(int)response.StatusCode} ({response.StatusCode}).");
        }
        catch (Exception ex)
        {
            return ConnectionValidationResult.Failure("Could not reach the ntfy server.", [ex.Message]);
        }
    }

    /// <summary>
    /// Publishes a notification using ntfy's JSON publish endpoint (POST to the server root with
    /// the topic in the body), which avoids the header-encoding pitfalls of the plain-text/header API.
    /// </summary>
    public static async Task<NtfyPublishResult> PublishAsync(
        HttpClient client,
        string serverUrl,
        string topic,
        string? accessToken,
        SendNotificationSettings settings,
        CancellationToken cancellationToken)
    {
        var payload = new Dictionary<string, object>
        {
            ["topic"] = topic,
            ["message"] = settings.Message
        };

        if (!string.IsNullOrWhiteSpace(settings.Title))
        {
            payload["title"] = settings.Title;
        }

        if (!string.IsNullOrWhiteSpace(settings.Tags))
        {
            var tags = settings.Tags.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (tags.Length > 0)
            {
                payload["tags"] = tags;
            }
        }

        // ntfy defaults to priority 3 (default) when omitted, so only send it when it differs
        var clampedPriority = Math.Clamp(settings.Priority, 1, 5);
        if (clampedPriority != 3)
        {
            payload["priority"] = clampedPriority;
        }

        if (!string.IsNullOrWhiteSpace(settings.Url))
        {
            var label = string.IsNullOrWhiteSpace(settings.UrlTitle) ? "Open" : settings.UrlTitle;
            payload["actions"] = new[]
            {
                new Dictionary<string, object>
                {
                    ["action"] = "view",
                    ["label"] = label,
                    ["url"] = settings.Url
                }
            };
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{serverUrl}/")
        {
            Content = JsonContent.Create(payload)
        };
        ApplyAuthHeader(request.Headers, accessToken);

        using var response = await client.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return new NtfyPublishResult(false, null, body, response.StatusCode);
        }

        var parsed = JsonSerializer.Deserialize<NtfyApiResponse>(body);
        return new NtfyPublishResult(true, parsed, body, response.StatusCode);
    }

    private static void ApplyAuthHeader(HttpRequestHeaders headers, string? accessToken)
    {
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}

/// <summary>
/// The outcome of an ntfy publish request.
/// </summary>
internal sealed record NtfyPublishResult(bool IsSuccess, NtfyApiResponse? Response, string RawBody, HttpStatusCode StatusCode);
