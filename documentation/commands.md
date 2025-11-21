[<- Back](../README.md)

# Saphira Commands

## Table of Contents

- [General Commands](#-general-commands)
- [Saphi Commands](#-saphi-commands)
- [Profile Commands](#-profile-commands)
- [Moderator Commands](#️-moderator-commands)

## 📋 General Commands

### `/help`
Lists all commands along with description and usage example.

**Usage:**
```
/help
```

**Response:**
- List of all existing commands

**Note:**
- Saphira will send the command help via DM, so DMs from server members must be enabled

### `/livestreams`
List all CTR livestreams from server members.

**Usage:**
```
/livestreams
```

**Response:**
- List of currently streaming members with their stream links
- Buttons to navigate between pages, 10 entries per page

**Requirements:**
- Bot needs "Server Members Intent" and "Presence Intent" enabled
- Only streams from users are detected whose Discord status is set to "Streaming"

### `/ping`
Check the bot's latency and uptime.

**Usage:**
```
/ping
```

**Response:**
- Bot latency in milliseconds
- Bot uptime

### `/server`
Get detailed information about the current server.

**Usage:**
```
/server
```

**Response:**
- General information (name, description, creation date, owner)
- Server statistics (members, emotes, roles, channels)
- Server limits (max bitrate, upload size, members)

## 🏁 Saphi Commands

### `/achievements`
Show a player's achievements and statistics.

**Usage:**
```
/achievements player:<player-name>
```

**Parameters:**
- `player` - The player's username

**Response:**
- Total points
- Course points
- Lap points
- First places
- Podium finishes

### `/leaderboard`
Get the leaderboard for a specific track and category.

**Usage:**
```
/leaderboard track:<track-name> category:<category-name>
```

**Parameters:**
- `track` - The custom track name (autocomplete available)
- `category` - The racing category (autocomplete available)

**Response:**
- Top times for the specified track and category
- Buttons to navigate between pages, 20 entries per page

### `/matchup`
Get the matchup between 2 players for a particular category.

**Usage:**
```
/matchup player1:<player-name> player2:<player-name> category:<category-name>
```

**Parameters:**
- `player1` - The name of the first player (autocomplete available)
- `player2` - The name of the second player (autocomplete available)
- `category` - The racing category (autocomplete available)

**Response:**
- A detailed matchup containing the winner score and loser score, as well as a comparison of personal best times on all common tracks

**Notes:**
- Player 1 and Player 2 must be 2 different players
- Player 1 and Player 2 must have at least one track in common that they have played

### `/pbs`
Get personal best times for a player across all tracks.

**Usage:**
```
/pbs player:<player-name>
```

**Parameters:**
- `player` - The player's username

**Response:**
- List of personal bests with track names, times, and ranks
- Buttons to navigate between pages, 20 entries per page

### `/tracks`
Get the list of all supported custom tracks.

**Usage:**
```
/tracks
```

**Response:**
- Complete list of custom tracks with their IDs
- Buttons to navigate between pages, 20 entries per page

## 👤 Profile Commands

### `/profile`
See your user profile.

**Usage:**
```
/profile
```

**Response:**
- Your Discord profile information

### `/toggle`
Toggle notification roles on or off.

**Usage:**
```
/toggle role:<role-name>
```

**Parameters:**
- `role` - The role to toggle (autocomplete available)

**Available Roles:**
- Saphi Updates
- Server Updates
- WR Feed

**Response:**
- Confirmation of role addition or removal

## 🛡️ Moderator Commands

> **Note:** These commands require the "Saphi Team" role.

### `/clearcache`
Clear the bot's in-memory cache.

**Usage:**
```
/clearcache
```

**Features:**
- Clears cached API data (categories, tracks, characters)
- Forces fresh data to be fetched on next request
- Useful when API data has been updated

**Permissions Required:**
- Saphi Team role

### `/dm`
Send a direct message to a user as the bot.

**Usage:**
```
/dm message:<message-text> user:<@user>
```

**Parameters:**
- `message` - The message text to send
- `user` - The user to DM

**Common Failure Reasons:**
- User has DMs disabled from server members
- User has blocked the bot
- User's privacy settings prevent DMs

**Permissions Required:**
- Saphi Team role

### `/post`
Send a message to a specific channel as the bot.

**Usage:**
```
/post message:<message-text> channel:<#channel>
```

**Parameters:**
- `message` - The message text to send
- `channel` - The target channel

**Permissions Required:**
- Saphi Team role

### `/purge`
Delete the last X messages in the current channel.

**Usage:**
```
/purge count:<number>
```

**Parameters:**
- `count` - Number of messages to delete (1-100)

**Limitations:**
- Only deletes messages less than 14 days old (Discord limitation)
- Maximum 100 messages per command

**Permissions Required:**
- Saphi Team role

### `/react`
Add a reaction to any message as the bot.

**Usage:**
```
/react emote:<emoji> messageId:<message-id>
```

**Parameters:**
- `emote` - The emoji or custom emote to react with
- `messageId` - The ID of the message to react to

**How to get a Message ID:**
1. Enable Developer Mode in Discord Settings → Advanced
2. Right-click on a message
3. Click "Copy ID"

**Supported Emotes:**
- Unicode emojis (😀, ✅, ⭐, etc.)
- Custom server emotes (`<:name:id>`)

**Permissions Required:**
- Saphi Team role

### `/verify`
Add the "Verified" role to a user.

**Usage:**
```
/verify user:<@user>
```

**Parameters:**
- `user` - The user to verify

**Permissions Required:**
- Saphi Team role
- Bot's role must be higher than "Verified" role in hierarchy
