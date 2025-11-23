using Discord;
using Saphira.Core.Cronjob;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Core.Logging;
using Saphira.Core.Security.Cooldown;

namespace Saphira.Discord.Cronjob;

[AutoRegister]
public class CooldownRegistryCleanupCronjob(CooldownService cooldownService, IMessageLogger logger) : ICronjob
{
    public async Task ExecuteAsync()
    {
        cooldownService.CleanupAllExpiredEntries();
        logger.Log(LogSeverity.Verbose, "Saphira", "Cleaned up all expired cooldown entries");
    }

    public TimeSpan GetInterval()
    {
        return TimeSpan.FromMinutes(30);
    }

    public TimeSpan GetStartDelay()
    {
        return TimeSpan.FromMinutes(1);
    }
}
