using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Arbor.AspNetCore.Host.Tests
{
    public class TestScheduledService : ScheduledService
    {
        public int Invokations { get; private set; }

        protected override Task RunAsync(DateTimeOffset currentTime, CancellationToken stoppingToken)
        {
            ++Invokations;

            return Task.CompletedTask;
        }

        public TestScheduledService([NotNull] ISchedule schedule, [NotNull] IScheduler scheduler) : base(schedule, scheduler)
        {
        }
    }
}