using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Arbor.App.Extensions.Application;
using Arbor.App.Extensions.Configuration;
using Xunit;

namespace Arbor.AspNetCore.Host.Tests
{
    public class AppInstancesTests
    {
        [Fact]
        public async Task CreateApp()
        {
            var instances = Array.Empty<object>();
            using App<TestStartup> _ = await App<TestStartup>.CreateAsync(new CancellationTokenSource(),
                Array.Empty<string>(), new Dictionary<string, string?>(), Array.Empty<Assembly>(), instances);
        }
    }
}