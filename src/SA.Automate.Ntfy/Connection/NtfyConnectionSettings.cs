using Umbraco.Automate.Core.Settings;

namespace SA.Automate.Ntfy.Connection;

/// <summary>
/// Stores the settings for an ntfy connection in Umbraco Automate.
/// Each connection targets a single topic on the configured (or overridden) ntfy server,
/// optionally authenticating with an access token for protected topics.
/// </summary>
public sealed class NtfyConnectionSettings
{
    /// <summary>
    /// The ntfy topic that notifications will be published to.
    /// </summary>
    [Field(
        Label = "Topic",
        Description = "The ntfy topic to publish notifications to.",
        SortOrder = 1)]
    public string Topic { get; set; } = string.Empty;

    /// <summary>
    /// Optionally overrides the globally configured ntfy server for this connection.
    /// Leave blank to use the default server (Umbraco:Automate:Providers:SA.Automate.Ntfy:ServerUrl).
    /// </summary>
    [Field(
        Label = "Server URL",
        Description = "Overrides the globally configured ntfy server for this connection. Leave blank to use the default.",
        SortOrder = 2)]
    public string? ServerUrl { get; set; }

    /// <summary>
    /// Optional access token used to authenticate against a protected topic.
    /// Leave blank for public topics. Marked as sensitive so the value is masked in the back office.
    /// </summary>
    [Field(
        Label = "Access Token",
        Description = "Optional. The ntfy access token used to authenticate against a protected topic. Leave blank for public topics.",
        IsSensitive = true,
        SortOrder = 3)]
    public string? AccessToken { get; set; }

    /// <summary>
    /// When enabled and no Access Token is set above, falls back to the globally configured default
    /// access token (Umbraco:Automate:Providers:SA.Automate.Ntfy:AccessToken). Leave disabled for
    /// public topics so no Authorization header is ever sent.
    /// </summary>
    [Field(
        Label = "Use default access token",
        Description = "If enabled and no Access Token is set above, falls back to the globally configured default access token in appsettings.json. Leave off for public topics.",
        SortOrder = 4)]
    public bool UseDefaultAccessToken { get; set; }
}
