using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arbor.AspNetCore.Host.Scheduling
{
    public abstract class ScheduledService
    {
        protected ScheduledService(ISchedule schedule, IScheduler scheduler)
        {
            Scheduler = scheduler;
            Schedule = schedule;

            scheduler.Add(schedule, Run);
        }

        public ISchedule Schedule { get; }

        public IScheduler Scheduler { get; }

        public virtual string Name => GetType().Name;

        public override string ToString() => Name;

        private async Task Run(DateTimeOffset dateTimeOffset) =>
            await RunAsync(dateTimeOffset, CancellationToken.None).ConfigureAwait(false);

        protected virtual Task RunAsync(DateTimeOffset currentTime, CancellationToken stoppingToken) =>
            Task.CompletedTask;
    }
}