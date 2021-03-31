using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Arbor.AspNetCore.Host.Sample
{
    public class SchedulerBackgroundService : BackgroundService
    {
        private readonly IScheduler _scheduler;
        private readonly ImmutableArray<ScheduledService> _services;
        private readonly ILogger _logger;

        public SchedulerBackgroundService(IScheduler scheduler, IEnumerable<ScheduledService> services, ILogger logger)
        {
            _scheduler = scheduler;
            _services = services.ToImmutableArray();
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information("Found {Schedules} schedules", _services);
            _logger.Information("Running scheduler {Scheduler}", _scheduler);

            return Task.CompletedTask;
        }
    }
}