[<- Back](../README.md)

# Requirements

## Configuration

A `config.json` file needs to be created with the following structure:

```json
{
  "BotToken": "",
  "GuildId": 0,
  "BotOwnerId": 0,
  "SaphiApiKey": "",
  "SaphiBaseUrl": "https://api.example.com"
}
```

- `BotToken` is the bot token you can acquire from the Discord Developer Portal
- `GuildId` is the internal ID of the server you want to run Saphira on
- `BotOwnerId` is the internal ID of the discord user who owns Saphira
- `SaphiApiKey` is an API key to access to leaderboard website API
- `SaphiBaseUrl` is the URL to the leaderboard website API

## Roles

Saphira needs a number of different roles to work properly:

- `Saphi Team` - This role needs to exist on the server and be given to team members or else access to moderator commands is unavailable
- `Verified` - This role is for verified users so that channel visibility can be restricted to users with that role
