using Arbor.App.Extensions.Application;
using Arbor.App.Extensions.Configuration;

namespace Arbor.AspNetCore.Host.Tests
{
    public class TestConfigureEnvironment : IConfigureEnvironment
    {
        public const int HttpPort = 15003;

        public void Configure(EnvironmentConfiguration environmentConfiguration)
        {
            environmentConfiguration.HttpEnabled = true;
            environmentConfiguration.HttpPort = HttpPort;
        }
    }
}