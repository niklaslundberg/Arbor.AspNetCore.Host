using System;
using System.Threading;
using System.Threading.Tasks;
using Arbor.AspNetCore.Host.Scheduling;
using Cronos;
using JetBrains.Annotations;
using Serilog;

namespace Arbor.AspNetCore.Host.Sample
{
    [UsedImplicitly]
    public class ScheduledLog : ScheduledService
    {
        private readonly ILogger _logger;

        public ScheduledLog([NotNull] IScheduler scheduler, ILogger logger) : base(
            new CronSchedule(CronExpression.Parse("* * * * * *", CronFormat.IncludeSeconds)),
            scheduler) => _logger = logger;

        public override string Name { get; } = "My custom scheduled service";

        protected override Task RunAsync(DateTimeOffset currentTime, CancellationToken stoppingToken)
        {
            _logger.Information("Running scheduled log at current time {CurrentTime}", currentTime);

            return Task.CompletedTask;
        }
    }
}