using Discord;
using Saphira.Discord.Logging;

namespace Saphira.Discord.Cronjob;

public class CronjobScheduler(IMessageLogger logger)
{
    private readonly List<ICronjob> _cronjobs = [];
    private readonly List<Timer> _timers = [];

    public void RegisterCronjob(ICronjob cronjob)
    {
        if (!_cronjobs.Contains(cronjob))
        {
            _cronjobs.Add(cronjob);
        }
    }

    public void UnregisterCronjob(ICronjob cronjob) => _cronjobs.Remove(cronjob);

    public void ScheduleCronjobs()
    {
        foreach (var cronjob in _cronjobs)
        {
            var timer = new Timer(
                state => ExecuteCronjob(cronjob),
                null,
                cronjob.GetStartDelay(),
                cronjob.GetInterval());

            _timers.Add(timer);
            logger.Log(LogSeverity.Verbose, "Saphira", $"Added timer for cronjob {cronjob}");
        }
    }

    private void ExecuteCronjob(ICronjob cronjob)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                logger.Log(LogSeverity.Verbose, "Saphira", $"Executing cronjob {cronjob} ...");
                await cronjob.ExecuteAsync();
            }
            catch (Exception ex)
            {
                logger.Log(LogSeverity.Error, "Saphira", $"Cronjob {cronjob} failed: {ex.Message}");
            }
        });
    }

    public void Dispose()
    {
        foreach (var timer in _timers)
        {
            timer.Dispose();
        }
    }
}
