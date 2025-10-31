﻿[<- Back](../README.md)

# Saphira Commands

## Table of Contents

- [General Commands](#-general-commands)
- [Saphi Commands](#-saphi-commands)
- [Profile Commands](#-profile-commands)
- [Moderator Commands](#️-moderator-commands)

## 📋 General Commands

### `/livestreams`
List all CTR livestreams from server members.

**Usage:**
```
/livestreams
```

**Response:**
- List of currently streaming members with their stream links

**Requirements:**
- Bot needs "Server Members Intent" and "Presence Intent" enabled

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
- Top 20 times for the specified track and category

### `/pbs`
Get personal best times for a player across all tracks.

**Usage:**
```
/pbs player:<player-name>
```

**Parameters:**
- `player` - The player's username

**Response:**
- Numbered list of personal bests with track names, times, and ranks

### `/tracks`
Get the list of all supported custom tracks.

**Usage:**
```
/tracks
```

**Response:**
- Complete list of custom tracks with their IDs

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
