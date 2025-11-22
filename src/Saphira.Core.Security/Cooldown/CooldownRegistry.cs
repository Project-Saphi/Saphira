using Discord.WebSocket;

namespace Saphira.Core.Security.Cooldown;

public class CooldownRegistry(TimeSpan cooldown)
{
    private readonly List<CooldownRegistryEntry> _entries = [];

    public void AddEntry(SocketGuildUser guildUser, string actionName, bool overrideIfExists = false)
    {
        var existingEntry = _entries.Where(e => e.GuildUser == guildUser && e.ActionName == actionName).FirstOrDefault();

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
        var existingEntry = _entries.Where(e => e.GuildUser == guildUser && e.ActionName == actionName).FirstOrDefault();

        if (existingEntry == null)
        {
            return true;
        }

        return _entries.Where(e => e.GuildUser == guildUser && e.ActionName == actionName && DateTime.Now - e.LastExecuted > cooldown).Any();
    }

    public void RemoveEntry(SocketGuildUser guildUser, string actionName)
    {
        var existingEntry = _entries.Where(e => e.GuildUser == guildUser && e.ActionName == actionName).FirstOrDefault();

        if (existingEntry == null)
        {
            throw new ArgumentException($"No cooldown entry for user {guildUser.Id} and action {actionName} exists.");
        }

        _entries.Remove(existingEntry);
    }

    public void CleanupExpiredEntries()
    {
        var expiredEntries = _entries.Where(e => DateTime.Now - e.LastExecuted > cooldown).ToList();

        foreach (var entry in expiredEntries)
        {
            _entries.Remove(entry);
        }
    }

    public int CountEntries(SocketGuildUser? guildUser = null, string? actionName = null)
    {
        var entries = _entries;

        if (guildUser != null)
        {
            entries = [.. entries.Where(e => e.GuildUser == guildUser)];
        }

        if (actionName != null)
        {
            entries = [.. entries.Where(e => e.ActionName == actionName)];
        }

        return entries.Count; 
    }
}
