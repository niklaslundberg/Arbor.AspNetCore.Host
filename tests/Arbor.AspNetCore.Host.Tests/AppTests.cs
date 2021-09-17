using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Arbor.App.Extensions.Logging;
using FluentAssertions;
using FluentAssertions.Extensions;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace Arbor.AspNetCore.Host.Tests
{
    public class AppTests
    {
        private readonly ITestOutputHelper _outputHelper;
        public AppTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        [Fact]
        public async Task RunAppShouldReturnExitCode0()
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            object[] instances = { new TestDependency() };

            var appTask = Task.Run(() => AppStarter<TestStartup>.StartAsync(Array.Empty<string>(),
                new Dictionary<string, string>(),
                cancellationTokenSource,
                instances: instances));

            await Task.Delay(1.Seconds());
            cancellationTokenSource.Cancel();

            int appExitCode = await appTask;

            using var logger = _outputHelper.CreateTestLogger();

            TempLogger.FlushWith(logger);

            appExitCode.Should().Be(0);
        }
    }
}