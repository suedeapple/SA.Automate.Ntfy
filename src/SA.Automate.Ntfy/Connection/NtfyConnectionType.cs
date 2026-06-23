using Microsoft.Extensions.Options;
using SA.Automate.Ntfy.Configuration;
using SA.Automate.Ntfy.Http;
using Umbraco.Automate.Core.Connections;

namespace SA.Automate.Ntfy.Connection;

/// <summary>
/// Defines the ntfy connection type for Umbraco Automate.
/// Stores the topic, an optional server override, and an optional access token per connection,
/// and validates reachability (and credentials, if a token is set) against the ntfy server before saving.
/// </summary>
[ConnectionType("ntfy", "ntfy",
    Description = "Connect to a public or protected ntfy topic",
    Group = "Messaging",
    Icon = "icon-plugin")]
public sealed class NtfyConnectionType : ConnectionTypeBase<NtfyConnectionSettings>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<NtfySettings> _ntfySettings;

    public NtfyConnectionType(
        ConnectionTypeInfrastructure infrastructure,
        IHttpClientFactory httpClientFactory,
        IOptionsMonitor<NtfySettings> ntfySettings)
        : base(infrastructure)
    {
        _httpClientFactory = httpClientFactory;
        _ntfySettings = ntfySettings;
    }

    /// <summary>
    /// Validates the connection with a lightweight reachability check against the topic's
    /// JSON endpoint, without publishing a visible notification.
    /// </summary>
    public override async Task<ConnectionValidationResult> ValidateAsync(
        object? settings,
        CancellationToken cancellationToken)
    {
        if (settings is not NtfyConnectionSettings typed)
            return ConnectionValidationResult.Failure("Connection settings are missing.");

        if (string.IsNullOrWhiteSpace(typed.Topic))
            return ConnectionValidationResult.Failure("Topic is required.");

        var serverUrl = NtfyRequestHelper.ResolveServerUrl(typed.ServerUrl, _ntfySettings.CurrentValue.ServerUrl);
        var accessToken = NtfyRequestHelper.ResolveAccessToken(typed.AccessToken, typed.UseDefaultAccessToken, _ntfySettings.CurrentValue.AccessToken);

        using var client = _httpClientFactory.CreateClient();

        return await NtfyRequestHelper.ValidateTopicAsync(client, serverUrl, typed.Topic, accessToken, cancellationToken);
    }
}
