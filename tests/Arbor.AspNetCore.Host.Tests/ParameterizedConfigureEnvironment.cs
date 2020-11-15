using Arbor.App.Extensions.DependencyInjection;
using Arbor.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Arbor.AspNetCore.Host.Tests
{
    public class ParameterizedConfigureEnvironment : IModule
    {
        public ParameterizedConfigureEnvironment(EnvironmentVariables environmentVariables)
        {
        }

        public IServiceCollection Register(IServiceCollection builder) => builder;
    }
}