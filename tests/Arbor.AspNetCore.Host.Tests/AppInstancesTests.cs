using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Arbor.AspNetCore.Host.Tests
{
    public class AppInstancesTests
    {
        [Fact]
        public async Task CreateApp()
        {
            object[] instances = Array.Empty<object>();

            using App<TestStartup> app = await App<TestStartup>.CreateAsync(new CancellationTokenSource(),
                Array.Empty<string>(),
                new Dictionary<string, string>(),
                Array.Empty<Assembly>(),
                instances);

            app.Should().NotBeNull();
        }
    }
}