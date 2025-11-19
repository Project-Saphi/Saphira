namespace Saphira.Discord.Event;

public interface IDiscordSocketClientEventSubscriber
{
    public void Register();

    public void Unregister();
}
