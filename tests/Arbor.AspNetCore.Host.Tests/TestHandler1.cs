using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;

namespace Arbor.AspNetCore.Host.Tests
{
    public sealed class TestHandler1 : INotificationHandler<TestNotificationA>, INotificationHandler<TestNotificationB>,
        IRequestHandler<TestRequest, Unit>
    {
        private readonly Guid _id;
        private readonly ILogger _logger;

        public TestHandler1(ILogger logger)
        {
            _id = Guid.NewGuid();
            _logger = logger;
            _logger.Information("Created test handler {Id}", ToString());
        }

        public List<IIdNotification> InvokedNotifications { get; } = new();

        public List<IRequest<Unit>> InvokedRequests { get; } = new();

        public Task Handle(TestNotificationA notification, CancellationToken cancellationToken)
        {
            InvokedNotifications.Add(notification);

            _logger.Information("Handler {Id} handling notification {NotificationId}",
                ToString(),
                notification.ToString());

            return Task.CompletedTask;
        }

        public Task Handle(TestNotificationB notification, CancellationToken cancellationToken)
        {
            InvokedNotifications.Add(notification);

            _logger.Information("Handler {Id} handling notification {NotificationId}",
                ToString(),
                notification.ToString());

            return Task.CompletedTask;
        }

        public Task<Unit> Handle(TestRequest request, CancellationToken cancellationToken)
        {
            InvokedRequests.Add(request);
            _logger.Information("Handler {Id} handling request {RequestId}", ToString(), request.ToString());

            return Task.FromResult(Unit.Value);
        }

        public override string ToString() => base.ToString() + " " + _id;
    }
}