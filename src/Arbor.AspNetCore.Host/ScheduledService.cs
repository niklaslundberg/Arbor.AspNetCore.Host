using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arbor.AspNetCore.Host
{
    public abstract class ScheduledService
    {
        private readonly IScheduler _scheduler;

        protected ScheduledService(ISchedule schedule, IScheduler scheduler)
        {
            _scheduler = scheduler;
            Schedule = schedule;

            scheduler.Add(schedule, Run);
        }

        public ISchedule Schedule { get; }

        private async Task Run(DateTimeOffset dateTimeOffset) => await RunAsync(dateTimeOffset, CancellationToken.None).ConfigureAwait(false);

        protected virtual Task RunAsync(DateTimeOffset currentTime, CancellationToken stoppingToken) =>
            Task.CompletedTask;
    }
}