using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Arbor.AspNetCore.Host.Sample
{
    public sealed class BackgroundTestHandler : BackgroundService,
        INotificationHandler<TestNotificationA>,
        INotificationHandler<TestNotificationB>
    {
        private readonly ILogger _logger;
        private Guid _id;

        public BackgroundTestHandler(ILogger logger)
        {
            _id = Guid.NewGuid();
            _logger = logger;
            _logger.Information("Created test handler {Id}", ToString());
        }

        public override string ToString() => base.ToString() +  " " + _id;

        public Task Handle(TestNotificationA notification, CancellationToken cancellationToken)
        {
            _logger.Information("Handler {Id} handling notification {NotificationId}", ToString(), notification.ToString());
            return Task.CompletedTask;
        }

        public Task Handle(TestNotificationB notification, CancellationToken cancellationToken)
        {
            _logger.Information("Handler {Id} handling notification {NotificationId}", ToString(), notification.ToString());
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));

                _logger.Information("Background task handler waiting");
            }
        }

        public void Test()
        {
            _logger.Information("Test was called on background service");
        }
    }
}