using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Arbor.App.Extensions.Time;
using Serilog;

namespace Arbor.AspNetCore.Host.Scheduling
{
    public sealed class Scheduler : IScheduler, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ICustomClock _clock;
        private readonly ConcurrentDictionary<ISchedule, DateTimeOffset?> _lastRun = new();
        private readonly ILogger _logger;

        private readonly ManualResetEventSlim _resetEvent = new(false);
        private readonly ConcurrentDictionary<ISchedule, OnTickAsync> _schedules = new();
        private readonly object _tickLock = new();
        private readonly ITimer? _timer;
        private bool _isDisposed;
        private bool _isDisposing;
        private bool _isRunning;

        public Scheduler(ICustomClock clock, ITimer timer, ILogger logger)
        {
            _clock = clock;
            _timer = timer;
            _logger = logger;
            _timer.Register(Schedule);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            if (_isDisposed || _isDisposing)
            {
                return;
            }

            _isDisposing = true;

            _timer?.Dispose();

            _cancellationTokenSource.Cancel();

            if (_isRunning)
            {
                _resetEvent.Wait();

                _schedules.Clear();
            }

            _cancellationTokenSource.Dispose();
            _isDisposed = true;
        }

        public bool Add(ISchedule schedule, OnTickAsync onTick)
        {
            CheckDisposed();

            return _schedules.TryAdd(schedule, onTick);
        }

        public ImmutableArray<ISchedule> Schedules => _schedules.Keys.ToImmutableArray();

        private void Schedule() => Task.Run(() => OnTickInternal(_clock.UtcNow()));

        private void CheckDisposed()
        {
            if (_isDisposing || _isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        private Task OnTickInternal(DateTimeOffset currentTime)
        {
            if (_isRunning)
            {
                return Task.CompletedTask;
            }

            lock (_tickLock)
            {
                if (_isRunning)
                {
                    return Task.CompletedTask;
                }

                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    _isRunning = false;
                    return Task.CompletedTask;
                }

                _isRunning = true;

                if (_schedules.IsEmpty)
                {
                    _isRunning = false;
                    return Task.CompletedTask;
                }

                var toRemove = new List<ISchedule>();

                foreach (var pair in _schedules)
                {
                    var nextTime = pair.Key.Next(currentTime);

                    if (nextTime is null)
                    {
                        toRemove.Add(pair.Key);
                        continue;
                    }

                    _lastRun.TryGetValue(pair.Key, out var lastRun);

                    if (lastRun.HasValue && lastRun.Value == nextTime)
                    {
                        continue;
                    }

                    double diff = (currentTime - nextTime.Value).TotalMilliseconds;

                    double absoluteDiff = Math.Abs(diff);

                    if (absoluteDiff < 50 && _lastRun.TryAdd(pair.Key, nextTime))
                    {
                        _logger.Verbose("Running schedule {Schedule}", pair.Key);
                        Task.Run(() => pair.Value(currentTime), _cancellationTokenSource.Token);
                    }
                    else if (nextTime > lastRun)
                    {
                        _lastRun.TryRemove(pair.Key, out _);
                    }
                }

                foreach (var schedule in toRemove)
                {
                    _schedules.TryRemove(schedule, out _);
                }

                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    _logger.Debug("Cancellation is requested, stopping schedules");
                    _isRunning = false;
                    _resetEvent.Set();
                }

                _isRunning = false;
            }

            return Task.CompletedTask;
        }
    }
}