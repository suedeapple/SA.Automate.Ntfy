using Microsoft.Extensions.Options;
using SA.Automate.Ntfy.Configuration;
using SA.Automate.Ntfy.Http;
using Umbraco.Automate.Core.Connections;

namespace SA.Automate.Ntfy.Connection;

/// <summary>
/// Defines the authenticated ntfy connection type for Umbraco Automate.
/// Stores the topic, an optional server override, and an access token per connection,
/// and validates reachability and credentials against the ntfy server before saving.
/// </summary>
[ConnectionType("ntfy-auth", "ntfy (Authenticated)",
    Description = "Connect to a protected ntfy topic using an access token",
    Group = "Messaging",
    Icon = "icon-lock")]
public sealed class NtfyAuthConnectionType : ConnectionTypeBase<NtfyAuthConnectionSettings>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<NtfySettings> _ntfySettings;

    public NtfyAuthConnectionType(
        ConnectionTypeInfrastructure infrastructure,
        IHttpClientFactory httpClientFactory,
        IOptionsMonitor<NtfySettings> ntfySettings)
        : base(infrastructure)
    {
        _httpClientFactory = httpClientFactory;
        _ntfySettings = ntfySettings;
    }

    /// <summary>
    /// Validates the connection with a lightweight, authenticated reachability check against
    /// the topic's JSON endpoint, without publishing a visible notification.
    /// </summary>
    public override async Task<ConnectionValidationResult> ValidateAsync(
        object? settings,
        CancellationToken cancellationToken)
    {
        if (settings is not NtfyAuthConnectionSettings typed)
            return ConnectionValidationResult.Failure("Connection settings are missing.");

        if (string.IsNullOrWhiteSpace(typed.Topic))
            return ConnectionValidationResult.Failure("Topic is required.");

        if (string.IsNullOrWhiteSpace(typed.AccessToken))
            return ConnectionValidationResult.Failure("Access token is required.");

        var serverUrl = NtfyRequestHelper.ResolveServerUrl(typed.ServerUrl, _ntfySettings.CurrentValue.ServerUrl);

        using var client = _httpClientFactory.CreateClient();

        return await NtfyRequestHelper.ValidateTopicAsync(client, serverUrl, typed.Topic, typed.AccessToken, cancellationToken);
    }
}
