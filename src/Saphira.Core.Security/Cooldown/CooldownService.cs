using Discord;
using Discord.WebSocket;
using Saphira.Discord.Logging;

namespace Saphira.Core.Security.Cooldown;

public class CooldownService(IMessageLogger logger)
{
    private readonly Dictionary<string, CooldownRegistry> _registries = [];

    public string CreateCooldownRegistryName(string commandName)
    {
        return $"slash_command_{commandName}";
    }

    public void AddRegistry(string registryName, TimeSpan cooldown, bool clearIfExists = false)
    {
        if (_registries.ContainsKey(registryName) && !clearIfExists)
        {
            throw new ArgumentException($"The cooldown registry '{registryName}' already exists.");
        }

        if (cooldown == TimeSpan.Zero)
        {
            throw new ArgumentException("The cooldown cannot be zero.");
        }

        _registries[registryName] = new CooldownRegistry(cooldown);
        logger.Log(LogSeverity.Verbose, "Saphira", $"Added new cooldown registry '{registryName}' with cooldown of {cooldown.TotalSeconds} seconds");
    }

    public void DeleteRegistry(string registryName)
    {
        if (!_registries.ContainsKey(registryName))
        {
            throw new ArgumentException($"No cooldown registry '{registryName}' exists.");
        }

        _registries.Remove(registryName);
        logger.Log(LogSeverity.Verbose, "Saphira", $"Removed cooldown registry '{registryName}'");
    }

    public void AddActionCooldown(string registryName, SocketGuildUser guildUser, string actionName, bool overrideIfExists = false)
    {
        if (!_registries.ContainsKey(registryName))
        {
            throw new ArgumentException($"No cooldown registry '{registryName}' exists.");
        }

        _registries[registryName].AddEntry(guildUser, actionName, overrideIfExists);
        logger.Log(LogSeverity.Verbose, "Saphira", $"Added new cooldown for user {guildUser.GlobalName} ({guildUser.Id}) for action '{actionName}' to registry '{registryName}'");
    }

    public bool CanUseActionAgain(string registryName, SocketGuildUser guildUser, string actionName)
    {
        if (!_registries.ContainsKey(registryName))
        {
            throw new ArgumentException($"No cooldown registry '{registryName}' exists.");
        }

        return _registries[registryName].IsEntryExpired(guildUser, actionName);
    }

    public void RemoveActionCooldown(string registryName, SocketGuildUser guildUser, string actionName)
    {
        if (!_registries.ContainsKey(registryName))
        {
            throw new ArgumentException($"No cooldown registry '{registryName}' exists.");
        }

        _registries[registryName].RemoveEntry(guildUser, actionName);
        logger.Log(LogSeverity.Verbose, "Saphira", $"Removed cooldown for user {guildUser.GlobalName} ({guildUser.Id}) for action '{actionName}' from registry '{registryName}'");
    }

    public void CleanupAllExpiredEntries()
    {
        foreach (var key in _registries.Keys)
        {
            _registries[key].CleanupExpiredEntries();
            logger.Log(LogSeverity.Verbose, "Saphira", $"Current size of registry '{key}: {_registries[key]}");
        }
    }
}
