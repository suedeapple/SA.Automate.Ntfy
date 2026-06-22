namespace SA.Automate.Ntfy.Actions;

/// <summary>
/// Output produced by the ntfy notification actions.
/// </summary>
public sealed class SendNotificationOutput
{
    /// <summary>
    /// Gets the message identifier assigned by ntfy, which can be used for tracking or debugging purposes.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Gets the unix timestamp at which ntfy created the message.
    /// </summary>
    public string? Time { get; init; }
}
