namespace Saphira.Discord.EventSubscriber
{
    public interface IDiscordEventSubscriber
    {
        public void Register();

        public void Unregister();
    }
}
