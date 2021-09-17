using System;
using System.Threading;
using System.Threading.Tasks;
using Arbor.AspNetCore.Host.Startup;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;

namespace Arbor.AspNetCore.Host.HealthChecks
{
    [UsedImplicitly]
    public class HealthBackgroundService : BackgroundService
    {
        private readonly HealthChecker? _healthChecker;
        private readonly StartupTaskContext? _startupTaskContext;

        public HealthBackgroundService(HealthChecker? healthChecker = null,
            StartupTaskContext? startupTaskContext = null)
        {
            _healthChecker = healthChecker;
            _startupTaskContext = startupTaskContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            if (_healthChecker is null || _startupTaskContext is null)
            {
                return;
            }

            try
            {
                while (!_startupTaskContext.IsCompleted)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }

                await _healthChecker.PerformHealthChecksAsync(stoppingToken);
            }
            catch (TaskCanceledException)
            {
                //
            }
        }
    }
}