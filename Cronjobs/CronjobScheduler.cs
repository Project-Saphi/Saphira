using Discord;
using Saphira.Util.Logging;

namespace Saphira.Cronjobs;

public class CronjobScheduler(IMessageLogger logger)
{
    private readonly List<ICronjob> _cronjobs = [];
    private readonly List<Timer> _timers = [];

    public void RegisterCronjob(ICronjob cronjob)
    {
        if (!_cronjobs.Contains(cronjob))
            _cronjobs.Add(cronjob);
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
        }
    }

    private void ExecuteCronjob(ICronjob cronjob)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await cronjob.ExecuteAsync();
            }
            catch (Exception ex)
            {
                logger.Log(LogSeverity.Error, "Saphira", $"Cronjob {cronjob.ToString()} failed: {ex.Message}");
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
