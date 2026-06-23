using Umbraco.Automate.Core.Settings;

namespace SA.Automate.Ntfy.Actions;

/// <summary>
/// Defines the configurable settings for the Send ntfy Notification actions in Umbraco Automate.
/// Each property maps to an editor field in the back office workflow step configuration.
/// </summary>
public class SendNotificationSettings
{
    /// <summary>An optional title displayed above the message in the notification.</summary>
    [Field(Label = "Title", Description = "An optional title to display above the message. Supports bindings.", SupportsBindings = true)]
    public string? Title { get; set; }

    /// <summary>The main body text of the notification. Required.</summary>
    [Field(Label = "Message", Description = "The main content of the notification to be sent. Supports bindings.", SortOrder = 1, SupportsBindings = true)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// A comma-separated list of tags/emoji shortcodes attached to the notification.
    /// See https://docs.ntfy.sh/publish/#tags-emojis for available values.
    /// </summary>
    [Field(Label = "Tags", Description = "A comma-separated list of tags or emoji shortcodes, e.g. \"warning,skull\". See https://docs.ntfy.sh/publish/#tags-emojis. Supports bindings.", SupportsBindings = true, SortOrder = 2)]
    public string? Tags { get; set; }

    /// <summary>
    /// An optional URL to attach as a "view" action button on the notification.
    /// </summary>
    [Field(Label = "URL", Description = "An optional URL to open via a button on the notification. Supports bindings.", SupportsBindings = true, SortOrder = 3)]
    public string? Url { get; set; }

    /// <summary>
    /// The label of the action button shown for the URL above. Defaults to "Open" if left blank.
    /// </summary>
    [Field(Label = "URL Title", Description = "The label of the button shown for the URL above. Defaults to \"Open\" if left blank. Supports bindings.", SupportsBindings = true, SortOrder = 4)]
    public string? UrlTitle { get; set; }

    
    /// <summary>
    /// Controls the urgency of the notification.
    /// 1 = min, 2 = low, 3 = default, 4 = high, 5 = max.
    /// Defaults to 3 (default).
    /// </summary>
    [Field(Label = "Priority", Description = "Set the notification priority level. Values: 1 (min), 2 (low), 3 (default), 4 (high), 5 (max).", SortOrder = 5)]
    public int Priority { get; set; } = 3;
}
