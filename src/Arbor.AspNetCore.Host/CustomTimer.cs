using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Arbor.App.Extensions.Time;

namespace Arbor.AspNetCore.Host
{
    public class CustomTimer : ITimer
    {
        private readonly ConcurrentDictionary<ISchedule,Func<DateTimeOffset,Task>> _taskFactory = new ();
        private readonly ConcurrentDictionary<ISchedule,DateTimeOffset> _running = new ();
        private readonly ICustomClock _clock;
        private readonly Timer _timer;

        public CustomTimer(ICustomClock clock)
        {
            _clock = clock;
            _timer = new Timer(DoDork, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void DoDork(object? state)
        {
            Task.Run(() => OnTickInternal(_clock.UtcNow()));
        }

        public async Task Add(ISchedule schedule, Func<DateTimeOffset, Task> factory)
        {
            _taskFactory.TryAdd(schedule, factory);
        }

        public Task OnTickInternal(DateTimeOffset currentTime)
        {
            if (_taskFactory.IsEmpty)
            {
                return Task.CompletedTask;
            }

            var toRemove = new List<ISchedule>();

            foreach (var pair in _taskFactory)
            {
                DateTimeOffset? nextTime = pair.Key.Next(currentTime);

                if (nextTime is null)
                {
                    toRemove.Add(pair.Key);
                    continue;
                }

                Console.WriteLine($"Current {currentTime}, Next {nextTime.Value}");
                bool executeNow = currentTime >= nextTime.Value;

                double diff = Math.Abs((nextTime.Value - currentTime).TotalMilliseconds);

                Console.WriteLine($"Diff {diff}");

                if (executeNow)
                {
                    _running.TryAdd(pair.Key, currentTime);
                    Task.Run(() => pair.Value(currentTime));
                    Console.WriteLine($"Execute now {executeNow} {diff:F1}");
                }
                else
                {
                    Console.WriteLine($"N IsInFuture {executeNow} {diff:F1}");
                }

                _running.TryRemove(pair.Key, out _);
            }

            foreach (var schedule in toRemove)
            {
                _taskFactory.TryRemove(schedule, out _);
            }

            return Task.CompletedTask;
        }
    }
}