﻿using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Serilog;

namespace Arbor.AspNetCore.Host.Sample
{
    [UsedImplicitly]
    public sealed class TestHandler : INotificationHandler<TestNotificationA>, INotificationHandler<TestNotificationB>,
        IRequestHandler<TestRequest, Unit>
    {
        private readonly Guid _id;
        private readonly ILogger _logger;

        public TestHandler(ILogger logger)
        {
            _id = Guid.NewGuid();
            _logger = logger;
            _logger.Information("Created test handler {Id}", ToString());
        }

        public Task Handle(TestNotificationA notification, CancellationToken cancellationToken)
        {
            _logger.Information("Handler {Id} handling notification {NotificationId}",
                ToString(),
                notification.ToString());

            return Task.CompletedTask;
        }

        public Task Handle(TestNotificationB notification, CancellationToken cancellationToken)
        {
            _logger.Information("Handler {Id} handling notification {NotificationId}",
                ToString(),
                notification.ToString());

            return Task.CompletedTask;
        }

        public Task<Unit> Handle(TestRequest request, CancellationToken cancellationToken)
        {
            _logger.Information("Handler {Id} handling request {RequestId}", ToString(), request.ToString());

            return Task.FromResult(Unit.Value);
        }

        public override string ToString() => $"{base.ToString()} {_id}";
    }
}