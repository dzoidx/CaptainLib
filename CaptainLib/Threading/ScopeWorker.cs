using System;
using System.Threading;
#if NETSTANDARD1_3 || NET45 || NET_4_6
using System.Threading.Tasks;
#endif
using System.Diagnostics;

namespace CaptainLib.Threading
{
    /// <summary>
    /// Base class for logic which executes 
    /// in separate thread while object is alive
    /// </summary>
    public abstract class ScopeWorker : IDisposable
    {
        public const int JOIN_TIMEOUT = 500;
        private volatile bool _disposed;
#if NET35
        private Thread _thread;
#else
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _task;
#endif

        protected ScopeWorker()
        {
#if NET35
            _thread = new Thread(ScopeLoop);
            _thread.IsBackground = true;
            _thread.Name = "ScopeWorker_" + GetType().Name;
            _thread.Start();
#else
            _task = Task.Run(() => ScopeLoop());
#endif
        }

        /// <summary>
        /// Implement this method
        /// Don't lock thread inside Update to support normal thread finalization on Dispose
        /// </summary>
#if NET35
        protected abstract void Update(float dt);
#else
        protected abstract void Update(float dt, CancellationToken cancellation);
#endif

        public void Dispose()
        {
            if (_disposed)
                return;
#if NET35
            if(_thread != Thread.CurrentThread)
                _thread.Join(JOIN_TIMEOUT);
            if (_thread.IsAlive)
                _thread.Abort();
#else
            _cancellationTokenSource.CancelAfter(JOIN_TIMEOUT);
#endif
            _disposed = true;
        }

        private void ScopeLoop()
        {
            var start = Stopwatch.GetTimestamp();
            while (!_disposed)
            {
                var curr = Stopwatch.GetTimestamp();
                var dt = (float)((double)(curr - start) / Stopwatch.Frequency);
#if NET35
                Update(dt);
#else
                Update(dt, _cancellationTokenSource.Token);
#endif
                start = curr;
            }
        }
    }
}
