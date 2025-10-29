[<- Back](../README.md)

# Requirements

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
  "GuildId": 0,
  "BotOwnerId": 0,
  "SaphiApiKey": "",
  "SaphiBaseUrl": "https://api.example.com",
  "MainChannelName": "general"
}
```

- `BotToken` is the bot token you can acquire from the Discord Developer Portal
- `GuildId` is the internal ID of the server you want to run Saphira on
- `BotOwnerId` is the internal ID of the discord user who owns Saphira
- `SaphiApiKey` is an API key to access to leaderboard website API
- `SaphiBaseUrl` is the URL to the leaderboard website API
- `MainChannelName` name of the main channel where Saphira will post some messages to (like the member count celebration)

## Roles

Saphira needs a number of different roles to work properly:

- `Saphi Team` - This role needs to exist on the server and be given to team members or else access to moderator commands is unavailable
- `Server Updates` - This role can be toggled on by users to receive pings regarding server updates
- `Saphi Updates` - This role can be toggled on by users to receive pings regarding Saphi updates
- `Verified` - This role is for verified users so that channel visibility can be restricted to users with that role
- `WR Feed` - This role can be toggled on by users to receive pings when new world records are posted in the submission feed