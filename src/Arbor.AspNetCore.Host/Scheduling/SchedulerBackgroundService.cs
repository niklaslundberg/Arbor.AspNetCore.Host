using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Arbor.AspNetCore.Host.Scheduling
{
    [UsedImplicitly]
    public class SchedulerBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IScheduler _scheduler;
        private readonly ImmutableArray<ScheduledService> _services;

        public SchedulerBackgroundService(IScheduler scheduler, IEnumerable<ScheduledService> services, ILogger logger)
        {
            _scheduler = scheduler;
            _services = services.ToImmutableArray();
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information("Found {@Schedules} schedules", _services.Select(service => service.Name).ToArray());
            _logger.Information("Running scheduler {Scheduler}", _scheduler);

            return Task.CompletedTask;
        }
    }
}