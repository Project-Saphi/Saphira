using Discord.WebSocket;

namespace Saphira.Discord.Pagination.Component;

public class PaginationComponentHandler
{
    private readonly Dictionary<Guid, PaginationState> _paginationStates = [];
    private readonly Dictionary<Guid, DateTime> _registrationTimes = [];
    private readonly Dictionary<Guid, CancellationTokenSource> _cleanupTokens = [];
    private readonly TimeSpan _timeout = TimeSpan.FromMinutes(5);

    public void RegisterPagination(Guid paginationId, PaginationState state)
    {
        if (_paginationStates.ContainsKey(paginationId))
        {
            throw new ArgumentException($"Pagination {paginationId} is already registered.");
        }

        _paginationStates[paginationId] = state;
        _registrationTimes[paginationId] = DateTime.UtcNow;

        var cts = new CancellationTokenSource();
        _cleanupTokens[paginationId] = cts;
        _ = Task.Delay(_timeout, cts.Token).ContinueWith(t =>
        {
            if (!t.IsCanceled)
            {
                UnregisterPagination(paginationId);
            }
        }, TaskScheduler.Default);
    }

    public void UnregisterPagination(Guid paginationId)
    {
        _paginationStates.Remove(paginationId);
        _registrationTimes.Remove(paginationId);

        if (_cleanupTokens.TryGetValue(paginationId, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
            _cleanupTokens.Remove(paginationId);
        }
    }

    public async Task HandlePreviousPage(Guid paginationId, SocketMessageComponent component)
    {
        if (!await ValidatePagination(paginationId, component))
        {
            return;
        }

        var state = _paginationStates[paginationId];
        var newPagination = new Pagination(paginationId, state.Pagination.GetPreviousPage(), state.Pagination.PageSize, state.Pagination.ItemCount);

        await state.UpdateCallback(component, newPagination);

        _paginationStates[paginationId] = state with { Pagination = newPagination };
    }

    public async Task HandleNextPage(Guid paginationId, SocketMessageComponent component)
    {
        if (!await ValidatePagination(paginationId, component))
        {
            return;
        }

        var state = _paginationStates[paginationId];
        var newPagination = new Pagination(paginationId, state.Pagination.GetNextPage(), state.Pagination.PageSize, state.Pagination.ItemCount);

        await state.UpdateCallback(component, newPagination);

        _paginationStates[paginationId] = state with { Pagination = newPagination };
    }

    private async Task<bool> ValidatePagination(Guid paginationId, SocketMessageComponent component)
    {
        if (!_paginationStates.TryGetValue(paginationId, out PaginationState? paginationState))
        {
            await component.RespondAsync("This pagination has expired.", ephemeral: true);
            return false;
        }

        if (_registrationTimes.TryGetValue(paginationId, out var registrationTime))
        {
            if (DateTime.UtcNow - registrationTime > _timeout)
            {
                UnregisterPagination(paginationId);
                await component.RespondAsync("This pagination has expired.", ephemeral: true);
                return false;
            }
        }

        if (paginationState.Filter != null)
        {
            var result = await paginationState.Filter(component);
            if (!result.Success)
            {
                await component.RespondAsync(result.CustomErrorMessage ?? "You cannot use this pagination.", ephemeral: true);
                return false;
            }
        }

        return true;
    }
}
