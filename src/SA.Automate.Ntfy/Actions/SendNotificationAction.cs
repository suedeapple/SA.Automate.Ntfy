using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SA.Automate.Ntfy.Configuration;
using SA.Automate.Ntfy.Connection;
using SA.Automate.Ntfy.Http;
using Umbraco.Automate.Core.Actions;

namespace SA.Automate.Ntfy.Actions;

/// <summary>
/// Umbraco Automate action that sends a notification to a public, unauthenticated ntfy topic.
/// Supports optional title, tags, and priority.
/// </summary>
[Action("ntfy.SendNotification", "Send ntfy Notification",
    Description = "Sends an ntfy Notification",
    Group = "Messaging",
    Icon = "icon-bell",
    ConnectionTypeAlias = "ntfy")]
public class SendNotificationAction : ActionBase<SendNotificationSettings, SendNotificationOutput>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SendNotificationAction> _logger;
    private readonly IOptionsMonitor<NtfySettings> _ntfySettings;

    public SendNotificationAction(
        ActionInfrastructure infrastructure,
        IHttpClientFactory httpClientFactory,
        ILogger<SendNotificationAction> logger,
        IOptionsMonitor<NtfySettings> ntfySettings)
        : base(infrastructure)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _ntfySettings = ntfySettings;
    }

    /// <summary>
    /// Executes the action by building the notification payload and posting it
    /// to the ntfy publish API. Returns a failed result on validation errors,
    /// missing configuration, or a non-success API response.
    /// </summary>
    public override async Task<ActionResult> ExecuteAsync(
        ActionContext context,
        CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<SendNotificationSettings>();

        if (string.IsNullOrWhiteSpace(settings.Message))
        {
            return ActionResult.Failed(
                new ArgumentException("Message is required."),
                StepRunErrorCategory.Validation);
        }

        var connectionSettings = context.Connection?.GetSettings<NtfyConnectionSettings>();

        if (connectionSettings == null || string.IsNullOrWhiteSpace(connectionSettings.Topic))
        {
            return ActionResult.Failed(
                new ArgumentException("ntfy connection settings are not configured properly."),
                StepRunErrorCategory.Authentication);
        }

        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var serverUrl = NtfyRequestHelper.ResolveServerUrl(connectionSettings.ServerUrl, _ntfySettings.CurrentValue.ServerUrl);

            var result = await NtfyRequestHelper.PublishAsync(
                httpClient, serverUrl, connectionSettings.Topic, null, settings, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogError("ntfy API request failed with status {StatusCode}: {ErrorContent}",
                    result.StatusCode, result.RawBody);

                return ActionResult.Failed(
                    new Exception($"ntfy API returned status {result.StatusCode}"),
                    StepRunErrorCategory.InvalidResponse);
            }

            _logger.LogInformation("Notification sent successfully to ntfy - Id: {Id}, Time: {Time}",
                result.Response?.Id, result.Response?.Time);

            return Success(new SendNotificationOutput
            {
                Id = result.Response?.Id ?? string.Empty,
                Time = result.Response?.Time.ToString() ?? string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to ntfy API");
            return ActionResult.Failed(ex, StepRunErrorCategory.InvalidResponse);
        }
    }
}
