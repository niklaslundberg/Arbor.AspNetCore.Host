using Arbor.App.Extensions.Application;
using Arbor.App.Extensions.Configuration;
using Arbor.AspNetCore.Host.Application;
using Arbor.KVConfiguration.Urns;
using FluentAssertions;
using Xunit;

namespace Arbor.AspNetCore.Host.Tests
{
    public class EnvironmentConfiguratorTests
    {
        [Fact]
        public void EnvironmentConfiguratorsShouldRespectAttributeOrder()
        {
            EnvironmentConfiguration configuration = new();
            ConfigurationInstanceHolder holder = new();

            holder.AddInstance(configuration);
            holder.AddInstance(new Configurator1());
            holder.AddInstance(new Configurator2());
            holder.AddInstance(new Configurator3());

            EnvironmentConfigurator.ConfigureEnvironment(holder);

            configuration.ApplicationName.Should().Be("app 3");
        }

        [Fact]
        public void EnvironmentConfiguratorsShouldRespectAttributeOrderRegardlessOfRegistrationOrder()
        {
            EnvironmentConfiguration configuration = new();
            ConfigurationInstanceHolder holder = new();

            holder.AddInstance(configuration);
            holder.AddInstance(new Configurator3());
            holder.AddInstance(new Configurator2());
            holder.AddInstance(new Configurator1());

            EnvironmentConfigurator.ConfigureEnvironment(holder);

            configuration.ApplicationName.Should().Be("app 3");
        }

        [RegistrationOrder(100)]
        private class Configurator1 : IConfigureEnvironment
        {
            public void Configure(EnvironmentConfiguration environmentConfiguration) =>
                environmentConfiguration.ApplicationName = "app 1";
        }

        private class Configurator2 : IConfigureEnvironment
        {
            public void Configure(EnvironmentConfiguration environmentConfiguration) =>
                environmentConfiguration.ApplicationName = "app 2";
        }

        [RegistrationOrder(200)]
        private class Configurator3 : IConfigureEnvironment
        {
            public void Configure(EnvironmentConfiguration environmentConfiguration) =>
                environmentConfiguration.ApplicationName = "app 3";
        }
    }
}