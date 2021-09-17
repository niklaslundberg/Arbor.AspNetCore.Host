using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Arbor.AspNetCore.Host.Messaging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace Arbor.AspNetCore.Host.Tests
{
    public class MediatorRegistrationTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public MediatorRegistrationTests(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;

        [Fact]
        public async Task NotificationsAndRequestsShouldBeHandledOnlyOncePerConcreteType()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(Logger.None);
            var assemblies = new List<Assembly> { GetType().Assembly };
            MediatorRegistrationHelper.Register(serviceCollection, assemblies);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var requiredService = serviceProvider.GetRequiredService<IMediator>();
            var testHandler1 = serviceProvider.GetRequiredService<TestHandler1>();
            var testHandler2 = serviceProvider.GetRequiredService<TestHandler2>();

            var id1 = Guid.NewGuid();
            await requiredService.Publish(new TestNotificationA(id1));
            var id2 = Guid.NewGuid();
            await requiredService.Publish(new TestNotificationA(id2));
            var id3 = Guid.NewGuid();
            await requiredService.Publish(new TestNotificationB(id3));
            var request1Id = Guid.NewGuid();
            var request2Id = Guid.NewGuid();
            var request3Id = Guid.NewGuid();

            await requiredService.Send(new TestRequest(request1Id));
            await requiredService.Send(new TestRequest(request2Id));
            await requiredService.Send(new TestRequest(request3Id));

            foreach (var testHandlerInvokedNotification in testHandler1.InvokedNotifications)
            {
                _testOutputHelper.WriteLine($"Notification: {testHandlerInvokedNotification}");
            }

            foreach (var request in testHandler1.InvokedRequests)
            {
                _testOutputHelper.WriteLine($"Request: {request}");
            }

            var ids = testHandler1.InvokedNotifications.Select(n => n.Id).ToArray();

            Assert.Equal(3, testHandler1.InvokedNotifications.Count);
            Assert.Equal(3, testHandler2.InvokedNotifications.Count);
            Assert.Contains(id1, ids);
            Assert.Contains(id2, ids);
            Assert.Contains(id3, ids);
            Assert.Equal(3, testHandler1.InvokedRequests.Count);
            Assert.Empty(testHandler2.InvokedRequests);
        }
    }
}