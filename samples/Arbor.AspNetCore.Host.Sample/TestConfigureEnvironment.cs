using Arbor.App.Extensions.Application;
using Arbor.App.Extensions.Configuration;
using JetBrains.Annotations;

namespace Arbor.AspNetCore.Host.Sample
{
    [UsedImplicitly]
    public class TestConfigureEnvironment : IConfigureEnvironment
    {
        public void Configure(EnvironmentConfiguration environmentConfiguration) =>
            environmentConfiguration.HttpEnabled = true;
    }
}