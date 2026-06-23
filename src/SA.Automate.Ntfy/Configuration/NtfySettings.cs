namespace SA.Automate.Ntfy.Configuration;

/// <summary>
/// Global ntfy settings bound from the <c>appsettings.json</c> configuration section
/// <c>Umbraco:Automate:Providers:SA.Automate.Ntfy</c>.
/// </summary>
public class NtfySettings
{
    /// <summary>
    /// The configuration section path used to bind these settings.
    /// </summary>
    public const string SectionName = "Umbraco:Automate:Providers:SA.Automate.Ntfy";

    /// <summary>
    /// The default ntfy server used by connections, unless a connection overrides it with its own Server URL.
    /// Defaults to the public ntfy.sh server. Point this at a self-hosted instance if you run your own.
    /// </summary>
    public string ServerUrl { get; set; } = "https://ntfy.sh";

    /// <summary>
    /// An optional default access token, used by connections that have "Use default access token"
    /// enabled and no Access Token of their own configured. Leave unset if you don't want a shared
    /// fallback token (e.g. when every connection targets a public topic).
    /// </summary>
    public string? AccessToken { get; set; }
}
