using Discord.WebSocket;

namespace Saphira.Core.Security.Cooldown;

public class CooldownRegistry(TimeSpan cooldown)
{
    private readonly List<CooldownRegistryEntry> _entries = [];

    public void AddEntry(SocketGuildUser guildUser, string actionName, bool overrideIfExists = false)
    {
        var existingEntry = _entries.FirstOrDefault(e => e.GuildUser == guildUser && e.ActionName == actionName);

        if (existingEntry != null && !overrideIfExists)
        {
            throw new ArgumentException($"There is already a cooldown entry for user {guildUser.Id} and action {actionName}");
        }

        if (existingEntry != null)
        {
            var updatedEntry = existingEntry with { LastExecuted = DateTime.Now };
            var index = _entries.IndexOf(existingEntry);

            _entries[index] = updatedEntry;
        }
        else
        {
            _entries.Add(new CooldownRegistryEntry(guildUser, actionName, DateTime.Now));
        }
    }

    public bool IsEntryExpired(SocketGuildUser guildUser, string actionName)
    {
        var existingEntry = _entries.FirstOrDefault(e => e.GuildUser == guildUser && e.ActionName == actionName);

        if (existingEntry == null)
        {
            return true;
        }

        return _entries.Any(e => e.GuildUser == guildUser && e.ActionName == actionName && DateTime.Now - e.LastExecuted > cooldown);
    }

    public void RemoveEntry(SocketGuildUser guildUser, string actionName)
    {
        var existingEntry = _entries.FirstOrDefault(e => e.GuildUser == guildUser && e.ActionName == actionName);

        if (existingEntry == null)
        {
            throw new ArgumentException($"No cooldown entry for user {guildUser.Id} and action {actionName} exists.");
        }

        _entries.Remove(existingEntry);
    }

    public void CleanupExpiredEntries()
    {
        _entries.RemoveAll(e => DateTime.Now - e.LastExecuted > cooldown);
    }

    public int CountEntries(SocketGuildUser? guildUser = null, string? actionName = null)
    {
        return _entries.Count(e =>
            (guildUser == null || e.GuildUser == guildUser) &&
            (actionName == null || e.ActionName == actionName));
    }
}
