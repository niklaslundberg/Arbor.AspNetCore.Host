using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Arbor.AspNetCore.Host.NoHttpSample
{
    [UsedImplicitly]
    public class TestBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;

        public TestBackgroundService(ILogger logger) => _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.Information("Running background service");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            _logger.Information("Background service completed");
        }
    }
}