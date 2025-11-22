[<- Back](../README.md)

# Setup

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

These settings have to be configured in the Discord [Developer Portal](https://discord.com/developers/).

## Configuration

A `config.json` file needs to be created with the following structure:

```json
{
  "BotToken": "",
  "BotOwnerId": 0,
  "MinimumLogLevel": 3,
  "GuildId": 0,
  "MainChannel": "",
  "SubmissionFeedChannel": "",
  "LivestreamsChannel": "",
  "CommandsAllowedChannels": [],
  "MaxAutocompleteSuggestions": 10,
  "SaphiApiKey": "",
  "SaphiApiBaseUrl": ""
}
```

- `BotToken` is the bot token you can acquire from the Discord Developer Portal
- `BotOwnerId` is the internal ID of the Discord user who owns Saphira
- `MinimumLogLevel` is the minimum log level of messages that will be logged (based on Discord.NET's `Discord.LogSeverity` enum)
- `GuildId` is the internal ID of the server you want to run Saphira on
- `MainChannel` is the name of the main channel where Saphira will post some messages to (like the member count celebration)
- `SubmissionFeedChannel` is the name of the channel where Saphira will post the newest submissions to
- `LivestreamsChannel` is the name of the channel where Saphira will post livestreams and suppress embeds for all messages
- `CommandsAllowedChannels` is a list of channels where users are allowed to use slash commands (unless they have the `Saphi Team` role)
- `MaxAutocompleteSuggestions` is the number of suggestions that are shown during command parameter autocompletion (hard-limited by Discord to at most 25)
- `SaphiApiKey` is an API key to access the leaderboard website API
- `SaphiApiBaseUrl` is the URL to the leaderboard website API

## Roles

Saphira needs a number of different roles to work properly:

- `Saphi Team` - This role needs to exist on the server and be given to team members or else access to moderator commands is unavailable
- `Server Updates` - This role can be toggled on by users to receive pings regarding server updates
- `Saphi Updates` - This role can be toggled on by users to receive pings regarding Saphi updates
- `Streaming` - This role is assigned to players who are streaming Crash Team Racing and have their Discord presence set to `Streaming`
- `Verified` - This role is for verified users so that channel visibility can be restricted to users with that role
- `WR Feed` - This role can be toggled on by users to receive pings when new world records are posted in the submission feed

## Permissions

Saphira requires the following Discord bot permissions to function properly:

- **Manage Messages** - Required for the `/purge` command to delete messages
- **Manage Roles** - Required for the `/verify` and `/toggle` commands to assign roles to users
- **Add Reactions** - Required for the `/react` command to add reactions to messages
- **Send Messages** - Required for posting messages via `/post` command and general bot responses
- **Embed Links** - Required for sending rich embed responses
- **Read Message History** - Required for various command functionalities

Make sure to grant these permissions when inviting the bot to your server, or assign them to the bot's role in your server settings.
