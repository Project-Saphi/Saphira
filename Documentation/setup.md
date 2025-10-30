[<- Back](../README.md)

# Requirements

## Table of Contents

- [System](#system)
- [Discord Application](#discord-application)
- [Configuration](#configuration)
- [Roles](#roles)
- [Permissions](#permissions)

## System

- .NET 9.0 or higher is required to run Saphira

## Discord Application

The bot application should be configured to have:

- The `Presence` Intent
- The `Server Members` Intent
- The `Message Content` Intent

These settings have to be configured in the Discord [developer portal](https://discord.com/developers/).

## Configuration

A `config.json` file needs to be created with the following structure:

```json
{
  "BotToken": "",
  "BotOwnerId": 0,
  "GuildId": 0,
  "MainChannelName": ""
  "SaphiApiKey": "",
  "SaphiApiBaseUrl": "https://api.example.com",
  "MaxLeaderboardEntries": 0
}
```

- `BotToken` is the bot token you can acquire from the Discord Developer Portal
- `BotOwnerId` is the internal ID of the Discord user who owns Saphira
- `GuildId` is the internal ID of the server you want to run Saphira on
- `MainChannelName` name of the main channel where Saphira will post some messages to (like the member count celebration)
- `SaphiApiKey` is an API key to access the leaderboard website API
- `SaphiApiBaseUrl` is the URL to the leaderboard website API
- `MaxLeaderboardEntries` is the number of entries that will be shown when using `/leaderboard`

## Roles

Saphira needs a number of different roles to work properly:

- `Saphi Team` - This role needs to exist on the server and be given to team members or else access to moderator commands is unavailable
- `Server Updates` - This role can be toggled on by users to receive pings regarding server updates
- `Saphi Updates` - This role can be toggled on by users to receive pings regarding Saphi updates
- `Verified` - This role is for verified users so that channel visibility can be restricted to users with that role
- `WR Feed` - This role can be toggled on by users to receive pings when new world records are posted in the submission feed

## Permissions

Saphira requires the following Discord bot permissions to function properly:

- **Manage Messages** - Required for the `/purge` command to delete messages
- **Manage Roles** - Required for the `/verify` command to assign roles to users
- **Kick Members** - Required for the `/kick` command
- **Ban Members** - Required for the `/ban` command
- **Moderate Members** - Required for the `/timeout` command to timeout users
- **Add Reactions** - Required for the `/react` command to add reactions to messages
- **Send Messages** - Required for posting messages via `/post` command and general bot responses
- **Embed Links** - Required for sending rich embed responses
- **Read Message History** - Required for various command functionalities

Make sure to grant these permissions when inviting the bot to your server, or assign them to the bot's role in your server settings.
