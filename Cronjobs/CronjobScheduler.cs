using Discord;
using Saphira.Util.Logging;

namespace Saphira.Cronjobs
{
    public class CronjobScheduler
    {
        private readonly IMessageLogger _logger;

        private List<ICronjob> _cronjobs = new List<ICronjob>();
        private List<Timer> _timers = new List<Timer>();

        public CronjobScheduler(IMessageLogger logger)
        {
            _logger = logger;
        }

        public void RegisterCronjob(ICronjob cronjob)
        {
            if (_cronjobs.Any(c => c == cronjob))
            {
                return;
            }

            _cronjobs.Add(cronjob);
        }

        public void UnregisterCronjob(ICronjob cronjob)
        {
            if (!_cronjobs.Any(c => c == cronjob))
            {
                return;
            }

            _cronjobs.Remove(cronjob);
        }

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
                    _logger.Log(LogSeverity.Error, "Saphira", $"Cronjob {cronjob.ToString()} failed: {ex.Message}");
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
}
