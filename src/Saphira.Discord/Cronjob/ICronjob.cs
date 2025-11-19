namespace Saphira.Discord.Cronjob;

public interface ICronjob
{
    public Task ExecuteAsync();

    public TimeSpan GetStartDelay();

    public TimeSpan GetInterval();
}
