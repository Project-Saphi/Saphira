using Microsoft.Extensions.Primitives;

namespace Saphira.Extensions.Caching
{
    public class CacheInvalidationService
    {
        private CancellationTokenSource _invalidationTokenSource = new CancellationTokenSource();
        private readonly object _lock = new object();

        public IChangeToken GetInvalidationToken()
        {
            return new CancellationChangeToken(_invalidationTokenSource.Token);
        }

        public void InvalidateAll()
        {
            lock (_lock)
            {
                var oldTokenSource = _invalidationTokenSource;
                _invalidationTokenSource = new CancellationTokenSource();
                oldTokenSource.Cancel();
                oldTokenSource.Dispose();
            }
        }
    }
}
