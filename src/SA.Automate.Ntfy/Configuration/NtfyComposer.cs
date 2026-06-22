using Microsoft.Extensions.DependencyInjection;
using SA.Automate.Ntfy.Actions;
using SA.Automate.Ntfy.Connection;
using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Connections;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace SA.Automate.Ntfy.Configuration;

/// <summary>
/// Registers all ntfy Automate services with the Umbraco dependency injection container.
/// This composer wires up the global settings, connection types, and available actions.
/// </summary>
public class NtfyComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // Bind the ntfy settings section from appsettings.json (e.g. ServerUrl)
        builder.Services.AddOptions<NtfySettings>()
            .BindConfiguration(NtfySettings.SectionName);

        // Register both ntfy connection types so they appear in Umbraco Automate connections
        builder.WithCollectionBuilder<ConnectionTypeCollectionBuilder>()
            .Add<NtfyConnectionType>()
            .Add<NtfyAuthConnectionType>();

        // Register the Send Notification actions so they are available in Umbraco Automate workflows
        builder.WithCollectionBuilder<ActionCollectionBuilder>()
            .Add<SendNotificationAction>()
            .Add<SendAuthenticatedNotificationAction>();
    }
}
