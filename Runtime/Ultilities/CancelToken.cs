using System;
using System.Threading;

namespace LazyCoder.Core
{
    public class CancelToken : IDisposable
    {
        private CancellationTokenSource _cancelTokenSource;
        private readonly object _lockObject = new object();
        private bool _disposed;

        public CancellationToken Token
        {
            get
            {
                lock (_lockObject)
                {
                    if (_disposed)
                        throw new ObjectDisposedException(nameof(CancelToken));
                    
                    if (_cancelTokenSource == null)
                        _cancelTokenSource = new CancellationTokenSource();

                    return _cancelTokenSource.Token;
                }
            }
        }

        public void Cancel()
        {
            lock (_lockObject)
            {
                if (_cancelTokenSource != null && !_cancelTokenSource.IsCancellationRequested)
                    _cancelTokenSource.Cancel();
            }
        }

        public void Dispose()
        {
            lock (_lockObject)
            {
                if (!_disposed)
                {
                    _cancelTokenSource?.Dispose();
                    _cancelTokenSource = null;
                    _disposed = true;
                }
            }
        }
        
        ~CancelToken()
        {
            Dispose();
        }
    }
}