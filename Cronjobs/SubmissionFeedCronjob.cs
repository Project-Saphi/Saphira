using Discord;
using Saphira.Extensions.DependencyInjection;
using Saphira.Saphi.Api;
using Saphira.Util.Logging;

namespace Saphira.Cronjobs
{
    [AutoRegister]
    public class SubmissionFeedCronjob : ICronjob
    {
        private readonly CachedClient _client;
        private readonly Configuration _configuration;
        private readonly IMessageLogger _logger;

        public SubmissionFeedCronjob(CachedClient client, Configuration configuration, IMessageLogger logger)
        {
            _client = client;
            _configuration = configuration;
            _logger = logger;
        }

        public Task ExecuteAsync()
        {
            _logger.Log(LogSeverity.Info, "Saphira", "Posting new submissions ...");
            return Task.CompletedTask;
        }

        public TimeSpan GetStartDelay()
        {
            return GetInterval();
        }

        public TimeSpan GetInterval()
        {
            return TimeSpan.FromMinutes(1);
        }
    }
}
