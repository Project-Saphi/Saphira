namespace Saphira.Core.Event;

public interface IEventSubscriber
{
    public void Register();

    public void Unregister();
}
