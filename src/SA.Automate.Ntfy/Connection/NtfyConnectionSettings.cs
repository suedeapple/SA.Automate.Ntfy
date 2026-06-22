using Umbraco.Automate.Core.Settings;

namespace SA.Automate.Ntfy.Connection;

/// <summary>
/// Stores the settings for a public, unauthenticated ntfy connection in Umbraco Automate.
/// Each connection targets a single topic on the configured (or overridden) ntfy server.
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
}
