using Discord.WebSocket;

namespace Saphira.Discord.Messaging.Pagination;

public class PaginationComponentHandler
{
    private readonly Dictionary<Guid, Func<SocketMessageComponent, Pagination, Task>> _callbacks = [];
    private readonly Dictionary<Guid, Pagination> _paginations = [];
    private readonly Dictionary<Guid, DateTime> _registrationTimes = [];
    private readonly Dictionary<Guid, CancellationTokenSource> _cleanupTokens = [];
    private readonly TimeSpan _timeout = TimeSpan.FromMinutes(5);

    public void RegisterComponent(Guid componentId, Pagination pagination, Func<SocketMessageComponent, Pagination, Task> callback)
    {
        if (_callbacks.ContainsKey(componentId))
        {
            throw new ArgumentException($"The component {componentId} is already registered.");
        }

        _callbacks[componentId] = callback;
        _paginations[componentId] = pagination;
        _registrationTimes[componentId] = DateTime.UtcNow;

        var cts = new CancellationTokenSource();
        _cleanupTokens[componentId] = cts;
        _ = Task.Delay(_timeout, cts.Token).ContinueWith(t =>
        {
            if (!t.IsCanceled)
            {
                UnregisterComponent(componentId);
            }
        }, TaskScheduler.Default);
    }

    public void UpdatePagination(Guid componentId, Pagination pagination)
    {
        if (!_paginations.ContainsKey(componentId))
        {
            throw new ArgumentException($"No pagination has been registered for component {componentId}.");
        }

        _paginations[componentId] = pagination;
    }

    public void UnregisterComponent(Guid componentId)
    {
        _callbacks.Remove(componentId);
        _paginations.Remove(componentId);
        _registrationTimes.Remove(componentId);

        if (_cleanupTokens.TryGetValue(componentId, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
            _cleanupTokens.Remove(componentId);
        }
    }

    public void UnregisterComponentGroup(Guid previousButtonId, Guid nextButtonId)
    {
        UnregisterComponent(previousButtonId);
        UnregisterComponent(nextButtonId);
    }

    public async Task HandleComponentInteraction(SocketMessageComponent component)
    {
        if (component?.Data?.CustomId == null)
        {
            return;
        }

        if (!Guid.TryParse(component.Data.CustomId, out var customId))
        {
            return;
        }

        if (!_callbacks.TryGetValue(customId, out var callback))
        {
            await component.RespondAsync("This pagination has expired.", ephemeral: true);
            return;
        }

        if (!_paginations.TryGetValue(customId, out var pagination))
        {
            await component.RespondAsync("This pagination has expired.", ephemeral: true);
            return;
        }

        if (_registrationTimes.TryGetValue(customId, out var registrationTime))
        {
            if (DateTime.UtcNow - registrationTime > _timeout)
            {
                UnregisterComponent(customId);
                await component.RespondAsync("This pagination has expired.", ephemeral: true);
                return;
            }
        }

        await callback(component, pagination);
    }
}
