# SA.Automate.Ntfy

[![Downloads](https://img.shields.io/nuget/dt/SA.Automate.Ntfy?color=cc9900)](https://www.nuget.org/packages/SA.Automate.Ntfy/)
[![NuGet](https://img.shields.io/nuget/vpre/SA.Automate.Ntfy?color=0273B3)](https://www.nuget.org/packages/SA.Automate.Ntfy)
[![GitHub license](https://img.shields.io/github/license/suedeapple/SA.Automate.Ntfy?color=8AB803)](https://github.com/suedeapple/SA.Automate.Ntfy/blob/main/LICENSE)

ntfy connection types and actions for [Umbraco Automate](https://github.com/umbraco/Umbraco.Automate). Send push notifications to [ntfy](https://ntfy.sh) as part of an automation workflow.

## What is ntfy?

[ntfy](https://ntfy.sh) (pronounced "notify") is a simple HTTP-based pub-sub notification service. You publish to a topic, and anyone subscribed to that topic, via the mobile app, web app, or desktop, receives the notification. It can be used against the public `ntfy.sh` server or against a self-hosted instance.

## What can this be used for?

This package is useful when you want instant operational alerts from Umbraco Automate workflows, for example:

- **Umbraco Commerce new orders**: notify a topic when a new order is placed.
- **Backoffice moderation tasks**: alert a topic when content is submitted for approval.
- **General team notifications**: route different events to different topics with different tags and priorities.

## Installation

```bash
dotnet add package SA.Automate.Ntfy
```

No further setup required. The composer registers itself automatically via Umbraco's `IComposer` discovery.

## Connection types

This package registers a single **ntfy** connection type, used for both public and protected topics:

- **Topic**: the ntfy topic to publish to.
- **Server URL**: optional. Overrides the globally configured server for this connection. Leave blank to use the default.
- **Access Token**: optional. Leave blank for public topics. If set, it's sent as an `Authorization: Bearer` header. Stored as a sensitive value and masked in the back office.
- **Use default access token**: optional toggle. If enabled and no Access Token is set above, falls back to the default access token configured in `appsettings.json`. Leave off for public topics, so no Authorization header is ever sent.

## Setup

### 1. (Optional) Configure defaults

By default, connections publish to the public `https://ntfy.sh` server with no access token. If you self-host ntfy, or want a shared access token, set defaults in your `appsettings.json` (or `appsettings.Production.json`); individual connections can still override the server URL, and only use the default token if they explicitly enable **Use default access token**:

```json
{
  "Umbraco": {
    "Automate": {
      "Providers": {
        "SA.Automate.Ntfy": {
          "ServerUrl": "https://ntfy.example.com",
          "AccessToken": "tk_..."
        }
      }
    }
  }
}
```

### 2. Create the connection in the backoffice

1. Go to **Automate → Connections** and create a new **ntfy** connection.
2. Give the connection a name and enter the **Topic** (and an **Access Token** if the topic is protected, or enable **Use default access token** to reuse the one configured in `appsettings.json`).
3. Optionally override the **Server URL** for this connection.
4. Click **Test connection** to verify. This performs a lightweight reachability check against the topic: it does not publish a visible notification, but note that it checks read access, which on strictly access-controlled self-hosted servers can differ from the write access actually needed to publish.

**Tip:** You can create multiple connections, with different topics, to send notifications to different channels.

## Usage

Add the **Send ntfy Notification** action to any automation and select the connection to use. Available fields:

| Field | Description |
|---|---|
| Title | An optional title to display above the message. Supports `${ binding }` expressions. |
| Message | The notification message. Supports `${ binding }` expressions. |
| Tags | A comma-separated list of tags or emoji shortcodes, e.g. `warning,skull`. See [ntfy's tag list](https://docs.ntfy.sh/publish/#tags-emojis). Supports `${ binding }` expressions. |
| URL | An optional URL to open when the notification itself is tapped. Supports `${ binding }` expressions. |
| Action Button URL | An optional URL to open via a button on the notification. Supports `${ binding }` expressions. |
| Action Button Label | The label of the button shown for the Action Button URL above. Defaults to `Open` if left blank. Supports `${ binding }` expressions. |
| Priority | The priority of the notification: `Min`, `Low`, `Default`, `High`, or `Max`. Defaults to `Default`. |

The action outputs an **Id** and **Time**, which can be referenced via bindings in later workflow steps.

## Compatibility

| Package version | Umbraco Automate | Umbraco CMS |
|---|---|---|
| 1.x | 17.x – 18.x | 17.x – 18.x |

## Links

- [Source code](https://github.com/suedeapple/SA.Automate.Ntfy)
- [Report an issue](https://github.com/suedeapple/SA.Automate.Ntfy/issues)
- [ntfy documentation](https://docs.ntfy.sh/)
