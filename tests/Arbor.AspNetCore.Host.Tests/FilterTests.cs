using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arbor.App.Extensions.Application;
using Arbor.App.Extensions.Logging;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace Arbor.AspNetCore.Host.Tests
{
    public class FilterTests
    {
        private readonly ITestOutputHelper _outputHelper;
        public FilterTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        [Fact]
        public async Task WhenNoValidationClassAttributeStatusCodeShouldNotBeBadRequest()
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            object[] instances = { new TestDependency() };

            var assemblies = ApplicationAssemblies.FilteredAssemblies();

            using var app = await App<TestStartup>.CreateAsync(cancellationTokenSource,
                Array.Empty<string>(),
                new Dictionary<string, string>(),
                assemblies,
                instances);

            int startExitCode = await app.RunAsync();

            startExitCode.Should().Be(0);

            var httpClientFactory = app.Host!.Services.GetRequiredService<IHttpClientFactory>();

            var httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.PostAsync($"http://localhost:{TestConfigureEnvironment.HttpPort}",
                new StringContent("{}", Encoding.UTF8, "application/json"));

            await Task.Delay(1.Seconds());

            cancellationTokenSource.Cancel();

            using var logger = _outputHelper.CreateTestLogger();

            TempLogger.FlushWith(logger);

            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        }

        [Fact]
        public async Task WhenNoValidationParameterAttributeStatusCodeShouldNotBeBadRequest()
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            object[] instances = { new TestDependency() };

            var assemblies = ApplicationAssemblies.FilteredAssemblies();

            using var app = await App<TestStartup>.CreateAsync(cancellationTokenSource,
                Array.Empty<string>(),
                new Dictionary<string, string>(),
                assemblies,
                instances);

            int startExitCode = await app.RunAsync();

            startExitCode.Should().Be(0);

            var httpClientFactory = app.Host!.Services.GetRequiredService<IHttpClientFactory>();

            var httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.PostAsync(
                "http://localhost:" + TestConfigureEnvironment.HttpPort + "/no-validation",
                new StringContent("{}", Encoding.UTF8, "application/json"));

            await Task.Delay(1.Seconds());

            cancellationTokenSource.Cancel();

            using var logger = _outputHelper.CreateTestLogger();

            TempLogger.FlushWith(logger);

            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        }

        [Fact]
        public async Task WhenDefaultValidationStatusCodeShouldBeBadRequest()
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            object[] instances = { new TestDependency() };

            var assemblies = ApplicationAssemblies.FilteredAssemblies();

            using var app = await App<TestStartup>.CreateAsync(cancellationTokenSource,
                Array.Empty<string>(),
                new Dictionary<string, string>(),
                assemblies,
                instances);

            int startExitCode = await app.RunAsync();

            startExitCode.Should().Be(0);

            var httpClientFactory = app.Host!.Services.GetRequiredService<IHttpClientFactory>();

            var httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.PostAsync(
                "http://localhost:" + TestConfigureEnvironment.HttpPort + "/validation",
                new StringContent("{}", Encoding.UTF8, "application/json"));

            await Task.Delay(1.Seconds());

            cancellationTokenSource.Cancel();

            using var logger = _outputHelper.CreateTestLogger();

            TempLogger.FlushWith(logger);

            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status400BadRequest);
        }
    }
}