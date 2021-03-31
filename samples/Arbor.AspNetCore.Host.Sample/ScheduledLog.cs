using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using JetBrains.Annotations;
using Serilog;

namespace Arbor.AspNetCore.Host.Sample
{
    public class ScheduledLog : ScheduledService
    {
        private readonly ILogger _logger;

        public ScheduledLog([NotNull] IScheduler scheduler, ILogger logger) : base(new CronSchedule(CronExpression.Parse("* * * * * *", CronFormat.IncludeSeconds)), scheduler) => _logger = logger;

        protected override Task RunAsync(DateTimeOffset currentTime, CancellationToken stoppingToken)
        {
            _logger.Information("Running scheduled log at current time {CurrentTime}", currentTime);

            return Task.CompletedTask;
        }
    }
}