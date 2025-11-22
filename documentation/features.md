[<- Back](../README.md)

# Features

Saphira has a bunch of built-in features that cannot be directly controlled via commands or settings. This document provides an overview about those features, along with conditions under which they are triggered.

## Table of Contents

- [Submission Feed](#submission-feed)
- [Livestreams](#livestreams)
- [Invite Link Blocker](#invite-link-blocker)
- [Restricted Content for new Members](#restricted-content-for-new-members)
- [Member Count Celebration](#member-count-celebration)

## Submission Feed

Every minute, Saphira will automatically post the newest submissions to a dedicated channel. In cases of new world records, Saphira will ping the `WR Feed` role.

## Livestreams

Saphira can detect players on the server who are streaming Crash Team Racing. Every time someone's Discord status changes to `Streaming` and they are playing CTR, Saphira will post the livestream information in a configurable channel (by default: `#livestreams`).

Players who are streaming CTR are also assigned the `Streaming` role. If they are no longer streaming CTR, the `Streaming` role is removed again.

**Note**: Only Twitch and YouTube are currently supported as streaming platforms.

## Invite Link Blocker

When sending a message in any text channel that contains a Discord invite link, the message will be removed. The only exception is when the sender has the `Saphi Team` role or when the sender is a Discord bot.

## Restricted Content for new Members

New members, who have been on the server for less than 12 hours, cannot send any message that contains a link, an attachment, an image or a video. The only exception is when the sender has the `Saphi Team` or the `Verified` role or when the sender is a Discord bot.

## Member Count Celebration

Every 100 members Saphira will post a message in the main channel to celebrate the current member count. The main channel can be configured in the [bot configuration](./setup.md).