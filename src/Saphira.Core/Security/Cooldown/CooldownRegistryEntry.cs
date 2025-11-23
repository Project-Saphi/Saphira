using Discord.WebSocket;

namespace Saphira.Core.Security.Cooldown;

public record CooldownRegistryEntry(
    SocketGuildUser GuildUser, 
    string ActionName, 
    DateTime LastExecuted
);