using Discord;
using Saphira.Extensions.DependencyInjection;
using Saphira.Saphi.Api;
using Saphira.Util.Logging;

namespace Saphira.Cronjobs;

[AutoRegister]
public class SubmissionFeedCronjob(CachedClient client, Configuration configuration, IMessageLogger logger) : ICronjob
{
    public Task ExecuteAsync()
    {
        logger.Log(LogSeverity.Info, "Saphira", "Posting new submissions ...");
        return Task.CompletedTask;
    }

    public TimeSpan GetStartDelay()
    {
        return GetInterval();
    }

    public TimeSpan GetInterval()
    {
        return TimeSpan.FromMinutes(5);
    }
}
