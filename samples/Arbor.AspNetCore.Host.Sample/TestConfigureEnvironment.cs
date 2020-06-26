using Arbor.App.Extensions.Application;
using Arbor.App.Extensions.Configuration;

namespace Arbor.AspNetCore.Host.Sample
{
    public class TestConfigureEnvironment : IConfigureEnvironment
    {
        public void Configure(EnvironmentConfiguration environmentConfiguration) =>
            environmentConfiguration.HttpEnabled = true;
    }
}