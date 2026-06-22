using Umbraco.Automate.Core.Settings;

namespace SA.Automate.Ntfy.Connection;

/// <summary>
/// Stores the settings for an authenticated ntfy connection in Umbraco Automate.
/// Each connection targets a single protected topic on the configured (or overridden) ntfy server,
/// using an access token to authenticate requests.
/// </summary>
public sealed class NtfyAuthConnectionSettings
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
    /// The access token used to authenticate against the protected topic.
    /// Marked as sensitive so the value is masked in the back office.
    /// </summary>
    [Field(
        Label = "Access Token",
        Description = "The ntfy access token used to authenticate against this topic.",
        IsSensitive = true,
        SortOrder = 3)]
    public string AccessToken { get; set; } = string.Empty;
}
