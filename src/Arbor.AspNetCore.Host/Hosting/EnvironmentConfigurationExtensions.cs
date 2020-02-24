using Arbor.App.Extensions.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

namespace Arbor.AspNetCore.Host.Hosting
{
    public static class EnvironmentConfigurationExtensions
    {
        public static IHostEnvironment ToHostEnvironment(this EnvironmentConfiguration environmentConfiguration) =>
            new HostingEnvironment { EnvironmentName = environmentConfiguration.EnvironmentName };
    }
}