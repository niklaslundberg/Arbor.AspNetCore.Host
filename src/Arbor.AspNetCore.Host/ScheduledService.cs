using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arbor.AspNetCore.Host
{
    public abstract class ScheduledService
    {
        private readonly IScheduler _time;

        protected ScheduledService(ISchedule schedule, IScheduler time)
        {
            _time = time;
            Schedule = schedule;

            time.Add(schedule, Run);
        }

        public ISchedule Schedule { get; }

        private async Task Run(DateTimeOffset dateTimeOffset) => await RunAsync(dateTimeOffset, CancellationToken.None);

        protected virtual Task RunAsync(DateTimeOffset currentTime, CancellationToken stoppingToken) =>
            Task.CompletedTask;
    }
}