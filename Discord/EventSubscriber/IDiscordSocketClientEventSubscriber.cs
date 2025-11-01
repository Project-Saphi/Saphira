namespace Saphira.Discord.EventSubscriber;

public interface IDiscordSocketClientEventSubscriber
{
    public void Register();

    public void Unregister();
}
