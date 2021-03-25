using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Arbor.App.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Arbor.AspNetCore.Host.Sample
{
    public class SchedulerModule : IModule
    {
        public IServiceCollection Register(IServiceCollection builder)
        {
            builder.AddSingleton<ITimer, SystemTimer>();
            builder.AddSingleton<IScheduler, Scheduler>();
            builder.AddSingleton<ScheduledService, ScheduledLog>();

            return builder;
        }
    }

    public class SchedulerBackgroundService : BackgroundService
    {
        private readonly IScheduler _scheduler;
        private readonly ILogger _logger;

        public SchedulerBackgroundService(IScheduler scheduler, IEnumerable<ScheduledService> services, ILogger logger)
        {
            _scheduler = scheduler;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information("Found {Schedules} schedules", _scheduler.Schedules.Length);

            return Task.CompletedTask;
        }
    }
}